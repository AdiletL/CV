using Gameplay.UI.ScreenSpace.Stats;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.ScreenSpace.Portrait
{
    public class UIPortrait : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private UIStats uiStats;
        
        private ShowStatsOnUI showStatsOnUI;

        public void Initialize()
        {
            diContainer.Inject(uiStats);
            uiStats.Initialize();
        }

        public void SetStatsController(IStatsController statsController)
        {
            showStatsOnUI = new ShowStatsOnUI(statsController, uiStats);
            showStatsOnUI.Initialize();
        }
    }
}