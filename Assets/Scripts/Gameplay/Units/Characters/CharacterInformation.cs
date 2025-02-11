
using Gameplay.UI.ScreenSpace.CreatureInformation;

namespace Unit.Character
{
    public class CharacterInformation : UnitInformation
    {
        private CharacterMainController characterMainController;
        
        public CharacterInformation(UnitController unitController) : base(unitController)
        {
            characterMainController = (CharacterMainController)unitController;
        }

        public override void UpdateData()
        {
            base.UpdateData();
            
            uiCreatureInformation.SetIcon(characterMainController.SO_CharacterInformation.Icon);
            uiCreatureInformation.AddText($"{StatsNames.NAME}: {characterMainController.SO_CharacterInformation.Name}");
            uiCreatureInformation.AddText($"{StatsNames.TYPE}: {StatsNames.GetTypesString(characterMainController.SO_CharacterInformation.EntityType)}");
            
            var characterHealth = characterMainController.GetComponentInUnit<IHealth>();
            uiCreatureInformation.SetHealth(characterHealth.CurrentHealth, characterHealth.MaxHealth);
            
            var characterEndurance = characterMainController.GetComponentInUnit<IEndurance>();
            uiCreatureInformation.SetEndurance(characterEndurance.CurrentEndurance, characterEndurance.MaxEndurance);
            
            var characterLevel = characterMainController.GetComponentInUnit<IUnitLevel>();
            uiCreatureInformation.AddText($"{StatsNames.LEVEL}: {characterLevel.CurrentLevel}");
            
            var characterExperience = characterMainController.GetComponentInUnit<IUnitExperience>();
            uiCreatureInformation.AddText($"{StatsNames.EXPERIENCE}: {characterExperience.CurrentExperience}");
            
            uiCreatureInformation.AddText($"{StatsNames.DAMAGE}: {characterMainController.TotalDamage()}");
            uiCreatureInformation.AddText($"{StatsNames.ATTACK_SPEED}: {characterMainController.TotalAttackSpeed()}");
            uiCreatureInformation.AddText($"{StatsNames.ATTACK_RANGE}: {(int)characterMainController.TotalAttackRange()}");
            uiCreatureInformation.SetDescription(characterMainController.SO_CharacterInformation.Description.ToString());
            
            uiCreatureInformation.Build();
        }
    }
}