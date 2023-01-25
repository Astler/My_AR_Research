using System;
using UnityEngine;

namespace Screens.FindDropZonesScreen
{
    public interface IFindTabBar
    {
        event Action<FindTabType> ClickedOnTab;

        void SetSelectedTab(FindTabType type);
    }

    public class FindTabBar : MonoBehaviour, IFindTabBar
    {
        [SerializeField] private Color selectedBorderColor;
        [SerializeField] private Color selectedBackgroundColor;
        [Space] [SerializeField] private Color unselectedBorderColor;
        [SerializeField] private Color unselectedBackgroundColor;

        [Space, SerializeField] private FindTab[] tabs;
        private TabViewInfo _selectedTabInfo, _unselectedTabInfo;

        public event Action<FindTabType> ClickedOnTab;

        public void SetSelectedTab(FindTabType type)
        {
            foreach (FindTab tab in tabs)
            {
                tab.ConfigureView(type == tab.Type ? _selectedTabInfo : _unselectedTabInfo);
            }
        }

        private void Awake()
        {
            _selectedTabInfo = new TabViewInfo
            {
                Border = selectedBorderColor,
                Background = selectedBackgroundColor
            };

            _unselectedTabInfo = new TabViewInfo
            {
                Border = unselectedBorderColor,
                Background = unselectedBackgroundColor
            };

            foreach (FindTab tab in tabs)
            {
                tab.Clicked += type => ClickedOnTab?.Invoke(type);
            }
        }
    }
}