
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
            uiCreatureInformation.SetName(characterMainController.SO_CharacterInformation.Name);
            uiCreatureInformation.SetType(characterMainController.SO_CharacterInformation.EntityType);
            
            var characterHealth = characterMainController.GetComponentInUnit<IHealth>();
            uiCreatureInformation.SetHealth(characterHealth.CurrentHealth, characterHealth.MaxHealth);
            
            var characterEndurance = characterMainController.GetComponentInUnit<IEndurance>();
            uiCreatureInformation.SetEndurance(characterEndurance.CurrentEndurance, characterEndurance.MaxEndurance);
            
            var characterLevel = characterMainController.GetComponentInUnit<IUnitLevel>();
            uiCreatureInformation.SetLevel(characterLevel.CurrentLevel);
            
            var characterExperience = characterMainController.GetComponentInUnit<IUnitExperience>();
            uiCreatureInformation.SetExperience(characterExperience.CurrentExperience);
            
            uiCreatureInformation.SetAmount(1);
            uiCreatureInformation.SetDamage(characterMainController.TotalDamage());
            uiCreatureInformation.SetAttackSpeed(characterMainController.TotalAttackSpeed());
            uiCreatureInformation.SetAttackRange((int)characterMainController.TotalAttackRange());
            uiCreatureInformation.SetDescription(characterMainController.SO_CharacterInformation.Description.ToString());
        }
    }
}