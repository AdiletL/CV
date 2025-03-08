
using Gameplay.UI.ScreenSpace.CreatureInformation;

namespace Gameplay.Unit.Character
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
            uiCreatureInformation.SetHealth(characterHealth.HealthStat.CurrentValue, characterHealth.HealthStat.MaximumValue);
            
            var characterEndurance = characterMainController.GetComponentInUnit<IEndurance>();
            uiCreatureInformation.SetEndurance(characterEndurance.EnduranceStat.CurrentValue, characterEndurance.EnduranceStat.MaximumValue);
            
            var characterLevel = characterMainController.GetComponentInUnit<ILevel>();
            uiCreatureInformation.AddText($"{StatsNames.LEVEL}: {characterLevel.LevelStat.CurrentValue}");
            
            var characterExperience = characterMainController.GetComponentInUnit<UnitExperience>();
            uiCreatureInformation.AddText($"{StatsNames.EXPERIENCE}: {characterExperience.ExperienceStat.CurrentValue}");

            uiCreatureInformation.SetDescription(characterMainController.SO_CharacterInformation.Description.ToString());
            
            uiCreatureInformation.Build();
        }
    }
}