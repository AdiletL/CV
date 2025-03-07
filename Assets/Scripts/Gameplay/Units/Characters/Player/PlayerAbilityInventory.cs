using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory;
using Gameplay.Ability;
using Gameplay.Common;
using Gameplay.UI.ScreenSpace.Inventory;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Cell;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(AbilityHandler))]
    public class PlayerAbilityInventory : MonoBehaviour, IInventory
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerAbilityInventory so_PlayerAbilityInventory;
        [SerializeField] private SO_PlayerAbilities so_PlayerAbilities;
        [SerializeField] private AssetReferenceGameObject uiAbilityInventoryPrefab;
        
        private AbilityHandler abilityHandler;
        private UIAbilityInventory uiAbilityInventory;
        private PlayerBlockInput playerBlockInput;

        private InputType baseBlockInput;
        private KeyCode[] abilityInventoryKeys;
        private Camera baseCamera;

        private Ability currentSelectedAbility;
        private Ability currentUseAbility;
        private Texture2D selectedAbilityCursor;
        
        private int maxSlot;
        
        private Dictionary<int?, Ability> slots;
        
        public bool IsFullInventory()
        {
            return slots != null && !slots.ContainsValue(null);
        }

        public void Initialize()
        {
            baseBlockInput = so_PlayerAbilityInventory.BaseBlockInputType;
            maxSlot = so_PlayerAbilityInventory.MaxSlot;
            abilityHandler = GetComponent<AbilityHandler>();
            playerBlockInput = playerController.PlayerBlockInput;
            baseCamera = playerController.BaseCamera;
            
            InitializeUIInventory();
            InitializeKeys();
            InitializeSlots();
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiAbilityInventoryPrefab).WaitForCompletion();
            uiAbilityInventory = handle.GetComponent<UIAbilityInventory>();
            diContainer.Inject(uiAbilityInventory);
            uiAbilityInventory.OnClickedLeftMouse += OnClickedLeftMouse;
            uiAbilityInventory.CreateCells(maxSlot);
        }

        private void InitializeKeys()
        {
            abilityInventoryKeys = new KeyCode[so_GameHotkeys.AbilityInventoryKeys.Length];
            for (int i = 0; i < abilityInventoryKeys.Length; i++)
                abilityInventoryKeys[i] = so_GameHotkeys.AbilityInventoryKeys[i];
        }

        private void InitializeSlots()
        {
            slots ??= new ();
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        public void AddAbility(Ability ability, Sprite icon)
        {
            if (slots.Values.Any(a => a != null && a.AbilityType == ability.AbilityType)) return;

            int? slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
            if (slotID == null) return;

            ability.SetInventorySlotID(slotID);
            abilityHandler.AddAbility(ability);
            slots[slotID.Value] = ability;
            ability.OnCountCooldown += OnCountCooldownAbility;
            ability.OnStartedCast += OnStartedCast;
            ability.OnFinishedCast += OnFinishedCast;

            var isSelectable = (ability.AbilityBehaviourID & AbilityBehaviour.Passive) == 0;
            uiAbilityInventory.AddAbility(slotID, icon, isSelectable, 0, 0);
        }

        public void RemoveAbility(int? slotID)
        {
            if (slotID == null) return;

            slots[slotID].OnCountCooldown -= OnCountCooldownAbility;
            slots[slotID].OnStartedCast -= OnStartedCast;
            slots[slotID].OnFinishedCast -= OnFinishedCast;
            
            abilityHandler.RemoveAbilityByID(slots[slotID].AbilityType, slotID);
            uiAbilityInventory.RemoveAbility(slotID);
            slots[slotID.Value] = null;
        }

        private void ClearSelectedAbility()
        {
            
            ExitAbility(currentSelectedAbility);
            currentSelectedAbility = null;
        }
        
        private void ClearUseAbility()
        {
            if(currentUseAbility == null) return;
            playerBlockInput.UnblockInput(slots[currentUseAbility.InventorySlotID].BlockedInputTypeID);
            ExitAbility(currentUseAbility);
            currentUseAbility = null;
        }
        
        private void OnCountCooldownAbility(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilityInventory.UpdateCooldown(slotID, current, max);
            uiAbilityInventory.UpdateSelectable(slotID, current <= 0);
        }

        private void OnClickedLeftMouse(int? slotID)
        {
            if (!playerBlockInput.IsInputBlocked(InputType.Ability))
            {
                switch (slots[slotID].AbilityBehaviourID)
                {
                    case AbilityBehaviour.NoTarget: slots[slotID].Enter(); break;
                    case AbilityBehaviour.PointTarget: 
                    case AbilityBehaviour.UnitTarget:
                        currentSelectedAbility = slots[slotID]; 
                        playerBlockInput.IsInputBlocked(baseBlockInput);
                        break;
                }
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var abilityFactory = (AbilityFactory)new AbilityFactoryBuilder()
                    .SetOwner(gameObject)
                    .Build();
                diContainer.Inject(abilityFactory);
                
                var newAbility = (DashAbility)abilityFactory.CreateAbility(so_PlayerAbilities.AbilityConfigData.DashConfig);
                diContainer.Inject(newAbility);
                newAbility.SetMoveControl(gameObject.GetComponent<IMoveControl>());
                newAbility.Initialize();
                AddAbility(newAbility, so_PlayerAbilities.AbilityConfigData.DashConfig.SO_BaseAbilityConfig.Icon);
            }
            
            if (currentUseAbility != null)
            {
                if (Input.anyKeyDown && !CheckInputOnUI.IsPointerOverUIObject())
                {
                    ClearUseAbility();
                }
            }
            
            if (currentSelectedAbility != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    switch (currentSelectedAbility.AbilityBehaviourID)
                    {
                        case AbilityBehaviour.PointTarget: PointTarget(); break;
                        case AbilityBehaviour.UnitTarget: UnitTarget(); break;
                    }
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    ClearSelectedAbility();
                }
            }

            HotkeysHandler();
        }

        private void HotkeysHandler()
        {
            for (int i = 0; i < abilityInventoryKeys.Length; i++)
            {
                if (Input.GetKeyDown(abilityInventoryKeys[i]))
                {
                    if (!playerBlockInput.IsInputBlocked(InputType.Ability))
                    {
                        switch (slots[i].AbilityBehaviourID)
                        {
                            case AbilityBehaviour.NoTarget: slots[i].Enter(); break;
                            case AbilityBehaviour.PointTarget: 
                            case AbilityBehaviour.UnitTarget:
                                currentSelectedAbility = slots[i]; 
                                playerBlockInput.IsInputBlocked(baseBlockInput);
                                break;
                        }
                    }
                }
            }
        }
        
        private void PointTarget()
        {
            Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CELL_LAYER))
            {
                if (hit.collider.TryGetComponent(out CellController cellController) &&
                    !cellController.IsBlocked())
                {
                    currentSelectedAbility.Enter(point: hit.point);
                    currentSelectedAbility = null;
                    playerBlockInput.UnblockInput(baseBlockInput);
                }
            }
        }

        private void UnitTarget()
        {
            Ray ray = baseCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CREEP_LAYER | Layers.PLAYER_LAYER))
            {
                if (hit.collider.TryGetComponent(out CharacterMainController characterMainController))
                {
                    currentSelectedAbility.Enter(target: characterMainController.gameObject);
                    currentSelectedAbility = null;
                    playerBlockInput.UnblockInput(baseBlockInput);
                }
            }
        }

        private void ExitAbility(Ability ability)
        {
            ability?.Exit();
        }

        private void OnStartedCast(int? slotID)
        {
            if(slotID == null) return;
            ExitAbility(currentUseAbility);
            playerBlockInput.BlockInput(slots[slotID].BlockedInputTypeID);
            currentUseAbility = slots[slotID];
        }
        private void OnFinishedCast(int? slotID)
        {
            if(slotID == null) return;
            playerBlockInput.UnblockInput(slots[slotID].BlockedInputTypeID);
            currentUseAbility = null;
        }
    }
}