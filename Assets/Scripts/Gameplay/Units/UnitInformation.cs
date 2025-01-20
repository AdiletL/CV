using Gameplay.UI.ScreenSpace.CreatureInformation;
using Zenject;

namespace Unit
{
    public abstract class UnitInformation
    {
        [Inject] public UICreatureInformation uiCreatureInformation;
        
        private UnitController unitController;
        
        public UnitInformation(UnitController unitController)
        {
            this.unitController = unitController;
        }

        public virtual void UpdateData()
        {
            uiCreatureInformation.ClearInformation();
        }
        
        public void Show() => uiCreatureInformation.Show();

        public void Hide() => uiCreatureInformation.Hide();
    }
}