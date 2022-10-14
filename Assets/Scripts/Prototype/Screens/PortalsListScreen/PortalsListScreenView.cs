using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens.PortalsListScreen
{
    public interface IPortalsListScreenView
    {
        void ShowScreen();
        PortalCardView GetCardPrefab();
        RectTransform GetListContainer();
    }
    
    public class PortalsListScreenView: ScreenView, IPortalsListScreenView
    {
        [SerializeField] private PortalCardView portalCardPrefab;
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button closeButton;

        protected override void OnAwake()
        {
            base.OnAwake();
            closeButton.onClick.AddListener(HideScreen);
        }

        public PortalCardView GetCardPrefab() => portalCardPrefab;
        public RectTransform GetListContainer() => listContainer;

        public void ShowScreen()
        {
            SetActive(true);
        }

        private void HideScreen()
        {
            SetActive(false);
        }
    }
}