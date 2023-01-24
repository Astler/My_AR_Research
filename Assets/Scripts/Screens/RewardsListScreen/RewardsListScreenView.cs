using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.RewardsListScreen
{
    public interface IRewardsListScreenView : IScreenView
    {
        RectTransform GetListContainer();
    }

    public class RewardsListScreenView : ScreenView, IRewardsListScreenView
    {
        [SerializeField] private Button okButton;
        [SerializeField] private RectTransform listContainer;

        public RectTransform GetListContainer() => listContainer;

        public void SetActive(bool active) => gameObject.SetActive(active);

        private void Awake()
        {
            okButton.ActionWithThrottle(CloseScreen);
        }
    }
}