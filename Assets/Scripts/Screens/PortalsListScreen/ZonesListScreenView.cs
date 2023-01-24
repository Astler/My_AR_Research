using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.PortalsListScreen
{
    public interface IZonesListScreenView : IScreenView
    {
        PortalCardView GetCardPrefab();
        RectTransform GetListContainer();
    }

    public class ZonesListScreenView : ScreenView, IZonesListScreenView
    {
        [SerializeField] private Button okButton;
        [SerializeField] private PortalCardView portalCardPrefab;
        [SerializeField] private RectTransform listContainer;

        public PortalCardView GetCardPrefab() => portalCardPrefab;
        public RectTransform GetListContainer() => listContainer;

        private void Awake()
        {
            okButton.ActionWithThrottle(CloseScreen);
        }
    }
}