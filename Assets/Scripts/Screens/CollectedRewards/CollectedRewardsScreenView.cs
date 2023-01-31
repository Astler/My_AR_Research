using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.CollectedRewards
{
    public interface ICollectedRewardsScreenView : IScreenView
    {
        event Action RefreshClicked;
        
        RectTransform CardsParent { get; }
        
        void SetIsAnyCollectedDrops(bool isAnyCollectedDrops);
    }

    public class CollectedRewardsScreenView : ScreenView, ICollectedRewardsScreenView
    {
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button refreshButton;
        [SerializeField] private GameObject noDropsView;

        public event Action RefreshClicked;
        
        public RectTransform CardsParent => listContainer;
        
        public void SetIsAnyCollectedDrops(bool isAnyCollectedDrops)
        {
            noDropsView.SetActive(!isAnyCollectedDrops);
        }
        
        private void Awake()
        {
            refreshButton.onClick.AddListener(() => RefreshClicked?.Invoke());
        }
    }
}