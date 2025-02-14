using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Factory;
using Gameplay.Skill;
using Gameplay.UI.ScreenSpace.Inventory;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Unit.Character.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        [Inject] private SO_GameConfig so_GameConfig;
        
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SO_PlayerInventory so_PlayerInventory;
        [SerializeField] private AssetReferenceGameObject uiInventoryPrefab;
        
        private UIInventory uiInventory;
        private SkillFactory skillFactory;
        private SkillHandler skillHandler;
        
        private ItemData currentSelectedItem;
        private List<ISkill> currentSkills = new();

        private Texture2D selectedItemCursor;
        private int maxCounItem;
        private int countInventaryID;
        private bool isSkillsCheck;
        
        private Dictionary<InputType, int> blockedInputs = new();
        private Dictionary<string, ItemData> currentItems = new();
        
        public bool IsFullInventory()
        {
            if(maxCounItem > currentItems.Count)
                return false;
            return true;
        }
        
        public bool TryGetItem(string name)
        {
            if (currentItems.ContainsKey(name))
                return true;

            return false;
        }

        public ItemData GetItem(string name)
        {
            return currentItems[name];
        }
        
        public bool IsInputBlocked(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }

        public void BlockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (!blockedInputs.ContainsKey(flag))
                    blockedInputs[flag] = 0;

                blockedInputs[flag]++;
            }
        }
        
        public void UnblockInput(InputType input)
        {
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.nothing || (input & flag) == 0) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }

        private SkillFactory CreateSkillFactory()
        {
            return new SkillFactoryBuilder(new SkillFactory())
                .SetBaseCamera(playerController.BaseCamera)
                .SetGameObject(gameObject)
                .Build();
        }
        
        public void OnEnable()
        {
            UIItem.OnItemSelected += OnItemSelected;
        }
        public void OnDisable()
        {
            UIItem.OnItemSelected -= OnItemSelected;
        }

        public void Initialize()
        {
            maxCounItem = so_PlayerInventory.MaxCountItem;
            selectedItemCursor = so_GameConfig.SelectedItemCursor;
            CreateUIInventory();
            skillFactory = CreateSkillFactory();
            diContainer.Inject(skillFactory);
            skillHandler = GetComponent<SkillHandler>();
        }

        private async void CreateUIInventory()
        {
            var handle = await Addressables.InstantiateAsync(uiInventoryPrefab);
            uiInventory = handle.GetComponent<UIInventory>();
            diContainer.Inject(uiInventory);
            uiInventory.SetMaxCountItem(maxCounItem);
        }

        public void AddItem(ItemData data)
        {
            if (!TryGetItem(data.Name))
            {
                if(countInventaryID < maxCounItem)
                    countInventaryID++;
                
                currentItems.Add(data.Name, data);
                currentItems[data.Name].SetID(countInventaryID);
                AddSkills(data.SkillConfigs, countInventaryID);
            }
            else
            {
                currentItems[data.Name].Amount += data.Amount;
            }
            
            uiInventory.AddItem(currentItems[data.Name]);
        }

        public void RemoveItem(ItemData data)
        {
            if(!TryGetItem(data.Name)) return;
            if(data.Amount > currentItems[data.Name].Amount) return;
            
            currentItems[data.Name].Amount -= 1;
            uiInventory.RemoveItem(currentItems[data.Name]);
            if (currentItems[data.Name].Amount <= 0)
            {
                currentItems.Remove(data.Name);
                foreach (var VARIABLE in data.SkillConfigs)
                    RemoveSkills(VARIABLE.SkillType, data.ID);
                
                if(countInventaryID > 0) countInventaryID--;
            }
        }

        private void AddSkills(List<SkillConfig> skillConfigs, int id)
        {
            foreach (var VARIEBLE in skillConfigs)
            {
                var newSkill = skillFactory.CreateSkill(VARIEBLE);
                if (newSkill != null)
                {
                    diContainer.Inject(newSkill);
                    newSkill.SetID(id);
                    newSkill.Initialize();
                    skillHandler.AddSkill(newSkill);
                }
            }
        }

        private void RemoveSkills(SkillType skillTypes, int id)
        {
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                if (skillTypes.HasFlag(skillType))
                    skillHandler.RemoveSkillByID(skillType, id);
            }
        }

        private void OnItemSelected(string itemName)
        {
            if (currentItems[itemName].IsCanSelect)
            {
                currentSelectedItem = currentItems[itemName];
                foreach (var VARIABLE in currentItems[itemName].SkillConfigs)
                {
                    var skill = skillHandler.GetSkill(VARIABLE.SkillType, currentSelectedItem.ID);
                    if(skill != null) currentSkills.Add(skill);
                }
                
                BlockInput(InputType.attack);
                Cursor.SetCursor(selectedItemCursor, Vector2.zero, CursorMode.Auto);
            }
        }

        public void ClearSelectedItem()
        {
            UnblockInput(InputType.attack);
            currentSelectedItem = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        
        private void Update()
        {
            if (currentSelectedItem != null && 
                Input.GetMouseButtonDown(0))
            {
                for (int i = currentSkills.Count - 1; i >= 0; i--)
                {
                    if (currentSkills[i].IsCanUseSkill())
                    {
                        skillHandler.Execute(currentSkills[i].SkillType, currentSelectedItem.ID);
                        if (i == 0)
                        {
                            RemoveItem(currentSelectedItem);
                            ClearSelectedItem();
                        }
                    }
                }
            }
        }
    }
}