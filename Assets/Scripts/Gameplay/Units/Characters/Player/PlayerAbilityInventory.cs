using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Factory;
using Gameplay.Ability;
using Gameplay.Effect;
using Gameplay.UI.ScreenSpace.Inventory;
using Gameplay.Unit.Cell;
using ScriptableObjects.Ability;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    [RequireComponent(typeof(AbilityHandler))]
    public class PlayerAbilityInventory : MonoBehaviour, IInventory, IActivatable
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        [Inject] private AbilityFactory abilityFactory;
        [Inject] private SO_BaseAbilityConfigContainer so_BaseAbilityConfigContainer;
        [Inject] private SO_GameRange so_GameRange;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerAbilityInventory so_PlayerAbilityInventory;
        [SerializeField] private SO_PlayerAbilities so_PlayerAbilities;
        [SerializeField] private SO_PlayerAbilityUsage so_PlayerAbilityUsage;
        [SerializeField] private AssetReferenceGameObject uiAbilityInventoryPrefab;
        
        private AbilityHandler abilityHandler;
        private UIAbilityInventory uiAbilityInventory;
        private PlayerBlockInput playerBlockInput;
        private PlayerAbilityUsageState playerAbilityUsageState;

        private InputType selectItemBlockInput;
        private KeyCode[] abilityInventoryHotkeys;
        private Camera baseCamera;
        private RangeDisplay rangeCastDisplay;

        private Ability.Ability currentSelectedAbility;
        private Texture2D selectedAbilityCursor;
        
        private int maxSlot;
        private bool isNextFrameFromUnblockInput;
        
        private Dictionary<int?, Ability.Ability> slots;
        
        public bool IsActive { get; private set; }

        
        public bool IsFullInventory()
        {
            return slots != null && !slots.ContainsValue(null);
        }

        public void Initialize()
        {
            selectItemBlockInput = so_PlayerAbilityInventory.SelectItemBlockInputType;
            maxSlot = so_PlayerAbilityInventory.MaxSlot;
            abilityHandler = GetComponent<AbilityHandler>();
            playerBlockInput = playerController.PlayerBlockInput;
            baseCamera = playerController.BaseCamera;
            
            InitializeUIInventory();
            InitializeHotkeys();
            InitializeSlots();
            InitializeUsageState();
        }

        private  void InitializeUIInventory()
        {
            var handle = Addressables.InstantiateAsync(uiAbilityInventoryPrefab).WaitForCompletion();
            uiAbilityInventory = handle.GetComponent<UIAbilityInventory>();
            diContainer.Inject(uiAbilityInventory);
            uiAbilityInventory.OnClickedLeftMouse += OnClickedLeftMouse;
            uiAbilityInventory.OnEnter += OnPointerEnter;
            uiAbilityInventory.OnExit += OnPointerExit;
            uiAbilityInventory.CreateCells(maxSlot);
        }

        private void InitializeHotkeys()
        {
            abilityInventoryHotkeys = new KeyCode[so_GameHotkeys.AbilityInventoryKeys.Length];
            for (int i = 0; i < abilityInventoryHotkeys.Length; i++)
                abilityInventoryHotkeys[i] = so_GameHotkeys.AbilityInventoryKeys[i];
        }

        private void InitializeSlots()
        {
            slots ??= new ();
            for (int i = 0; i < maxSlot; i++)
                slots.Add(i, null);
        }

        private void InitializeUsageState()
        {
            playerController.PlayerStateFactory.SetPlayerAbilityUsageConfig(so_PlayerAbilityUsage);
            playerAbilityUsageState = (PlayerAbilityUsageState)playerController.PlayerStateFactory.CreateState(typeof(PlayerAbilityUsageState));
            diContainer.Inject(playerAbilityUsageState);
            playerAbilityUsageState.Initialize();
            playerController.StateMachine.AddStates(playerAbilityUsageState);
        }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void AddAbility(Ability.Ability ability, Sprite icon)
        {
            if (slots.Values.Any(a => a != null && a.AbilityTypeID == ability.AbilityTypeID)) return;

            int? slotID = slots.FirstOrDefault(pair => pair.Value == null).Key;
            if (slotID == null) return;

            ability.SetInventorySlotID(slotID);
            abilityHandler.AddAbility(ability);
            slots[slotID.Value] = ability;
            ability.OnCountCooldown += OnCountCooldownAbility;

            var isSelectable = (ability.AbilityBehaviourID & AbilityBehaviour.Passive) == 0;
            uiAbilityInventory.AddAbility(slotID, icon, isSelectable, 0, 0);
        }

        public void RemoveAbility(int? slotID)
        {
            if (slotID == null) return;

            slots[slotID].OnCountCooldown -= OnCountCooldownAbility;
            
            abilityHandler.RemoveAbilityByID(slots[slotID].AbilityTypeID, slotID);
            uiAbilityInventory.RemoveAbility(slotID);
            slots[slotID.Value] = null;
        }

        private void CreateRangeCastDisplay()
        {
            if(rangeCastDisplay) return;
            var newGameObject = Addressables.InstantiateAsync(so_GameRange.CastPrefab, playerController.VisualParent.transform).WaitForCompletion();
            newGameObject.transform.localPosition = Vector3.zero;
            rangeCastDisplay = newGameObject.GetComponent<RangeDisplay>();
        }
        
        private void OnPointerEnter(int? slotID)
        {
            if(slotID == null || slots[slotID] == null) return;
            CreateRangeCastDisplay();
            rangeCastDisplay.SetRange(slots[slotID].Range);
            rangeCastDisplay.ShowRange();
        }

        private void OnPointerExit()
        {
            rangeCastDisplay?.HideRange();
        }
        
        private void ClearSelectedAbility()
        {
            ExitAbility(currentSelectedAbility);
            currentSelectedAbility = null;
        }

        private void OnCountCooldownAbility(int? slotID, float current, float max)
        {
            if(slotID == null) return;
            uiAbilityInventory.SetCooldown(slotID, current, max);
            uiAbilityInventory.SetSelectable(slotID, current <= 0);
        }

        private void OnClickedLeftMouse(int? slotID)
        {
            if(slotID == null) return;
            SelectAbilityHandler(slots[slotID]);
        }

        private void SelectAbilityHandler(Ability.Ability ability)
        {
            if (!playerBlockInput.IsInputBlocked(InputType.Ability))
            {
                switch (ability.AbilityBehaviourID)
                {
                    case AbilityBehaviour.NoTarget: 
                        ability.Enter();
                        playerAbilityUsageState.SetAbility(ability);
                        playerController.StateMachine.ExitOtherStates(playerAbilityUsageState.GetType());
                        break;
                    case AbilityBehaviour.PointTarget:
                    case AbilityBehaviour.UnitTarget:
                        currentSelectedAbility = ability; 
                        playerBlockInput.IsInputBlocked(selectItemBlockInput);
                        CreateRangeCastDisplay();
                        rangeCastDisplay.SetRange(currentSelectedAbility.Range);
                        rangeCastDisplay.ShowRange();
                        break;
                }
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var newAbility = (DashAbility)abilityFactory.CreateAbility(so_PlayerAbilities.AbilityConfigData.DashConfig);
                diContainer.Inject(newAbility);
                newAbility.SetMoveControl(gameObject.GetComponent<IMoveControl>());
                newAbility.SetGameObject(gameObject);
                newAbility.Initialize();
                AddAbility(newAbility, so_BaseAbilityConfigContainer.GetAbilityConfig(so_PlayerAbilities.AbilityConfigData.DashConfig.AbilityTypeID).Icon);
            }
            
            if (isNextFrameFromUnblockInput)
            {
                playerBlockInput.UnblockInput(selectItemBlockInput);
                isNextFrameFromUnblockInput = false;
            }
            
            /*if (currentUseAbility != null)
            {
                if (Input.anyKeyDown && playerBlockInput.IsInputBlocked(useItemBlockInput))
                {
                    ClearUseAbility();
                }
            }*/
            
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
                    isNextFrameFromUnblockInput = true;
                    rangeCastDisplay.HideRange();
                }
            }

            HotkeysHandler();

            if (Input.GetKeyDown(KeyCode.J))
            {
                var disableEffect = new DisableEffect(so_PlayerAbilities.EffectConfigData.DisableConfigs[0]);
                diContainer.Inject(disableEffect);
                GetComponent<PlayerDisableController>().ActivateDisable(disableEffect);
                disableEffects.Add(disableEffect);
                disableEffect.ApplyEffect();
            }
            else if(Input.GetKeyDown(KeyCode.K))
            {
                if (disableEffects.Count > 0)
                {
                    GetComponent<PlayerDisableController>().DeactivateDisable(disableEffects[^1]);
                    disableEffects.RemoveAt(disableEffects.Count - 1);
                }
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                var disableEffect = new DisableEffect(so_PlayerAbilities.EffectConfigData.DisableConfigs[1]);
                diContainer.Inject(disableEffect);
                GetComponent<PlayerDisableController>().ActivateDisable(disableEffect);
                disableEffects.Add(disableEffect);
                disableEffect.ApplyEffect();
            }
        }

        private List<DisableEffect> disableEffects = new List<DisableEffect>();
        
        private void HotkeysHandler()
        {
            for (int i = 0; i < abilityInventoryHotkeys.Length; i++)
            {
                if (Input.GetKeyDown(abilityInventoryHotkeys[i]))
                {
                    SelectAbilityHandler(slots[i]);
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
                    if(!Calculate.Distance.IsDistanceToTargetSqr(transform.position, hit.point, 
                           currentSelectedAbility.Range * currentSelectedAbility.Range))
                        return;
                    
                    currentSelectedAbility.Enter(point: hit.point);
                    playerAbilityUsageState.SetAbility(currentSelectedAbility);
                    playerController.StateMachine.ExitOtherStates(playerAbilityUsageState.GetType());
                    currentSelectedAbility = null;
                    isNextFrameFromUnblockInput = true;
                    rangeCastDisplay.HideRange();
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
                    if(!Calculate.Distance.IsDistanceToTargetSqr(transform.position, characterMainController.transform.position,
                           currentSelectedAbility.Range * currentSelectedAbility.Range))
                        return;
                    
                    currentSelectedAbility.Enter(target: characterMainController.gameObject);
                    playerAbilityUsageState.SetAbility(currentSelectedAbility);
                    playerController.StateMachine.ExitOtherStates(playerAbilityUsageState.GetType());
                    currentSelectedAbility = null;
                    isNextFrameFromUnblockInput = true;
                    rangeCastDisplay.HideRange();
                }
            }
        }

        private void ExitAbility(Ability.Ability ability)
        {
            ability?.Exit();
        }
    }
}