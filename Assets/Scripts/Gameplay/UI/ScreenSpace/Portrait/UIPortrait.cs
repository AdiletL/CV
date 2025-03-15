using Gameplay.UI.ScreenSpace.Stats;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.ScreenSpace.Portrait
{
    public class UIPortrait : MonoBehaviour
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private UIStats uiStats;
        
        private MediatorStatsAndUI mediatorStatsAndUI;

        public void Initialize()
        {
            diContainer.Inject(uiStats);
            uiStats.Initialize();
        }

        public void SetStatsController(IStatsController statsController)
        {
            mediatorStatsAndUI = new MediatorStatsAndUI(statsController, uiStats);
            mediatorStatsAndUI.Initialize();
        }
    }
}