using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.FindDropZonesScreen
{
    public interface IZonesListScreenView : IScreenView
    {
        event Action MapToPlayerPositionClicked;
        event Action LaunchArClicked;
        
        IFindTabBar TabBar { get; }
        RectTransform CardsParent { get; }
        
        void ShowContentByTab(FindTabType selectedTab);
    }

    public class ZonesListScreenView : ScreenView, IZonesListScreenView
    {
        [SerializeField] private FindTabBar findTabBar;
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button toMeButton;
        [SerializeField] private Button launchArButton;
        [Space] 
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject mapContent;

        public event Action MapToPlayerPositionClicked;
        public event Action LaunchArClicked;
        
        public IFindTabBar TabBar => findTabBar;
        public RectTransform CardsParent => listContainer;
        
        public void ShowContentByTab(FindTabType selectedTab)
        {
            bool isMapMode = selectedTab == FindTabType.Map;
            
            background.SetActive(!isMapMode);
            scrollView.gameObject.SetActive(!isMapMode);
            mapContent.SetActive(isMapMode);
        }

        private void Awake()
        {
            toMeButton.ActionWithThrottle(() => MapToPlayerPositionClicked?.Invoke());
            launchArButton.ActionWithThrottle(() => LaunchArClicked?.Invoke());
        }
    }
}