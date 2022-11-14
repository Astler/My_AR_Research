using UnityEngine;

namespace Screens.PortalsListScreen
{
    public interface IPortalsListScreenView
    {
        PortalCardView GetCardPrefab();
        RectTransform GetListContainer();
    }
    
    public class ZonesListView: MonoBehaviour, IPortalsListScreenView
    {
        [SerializeField] private PortalCardView portalCardPrefab;
        [SerializeField] private RectTransform listContainer;

        public PortalCardView GetCardPrefab() => portalCardPrefab;
        public RectTransform GetListContainer() => listContainer;
    }
}