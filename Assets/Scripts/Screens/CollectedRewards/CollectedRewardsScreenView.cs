using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.CollectedRewards
{
    public interface ICollectedRewardsScreenView : IScreenView
    {
        event Action RefreshClicked;
        RectTransform CardsParent { get; }
    }

    public class CollectedRewardsScreenView : ScreenView, ICollectedRewardsScreenView
    {
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button refreshButton;

        public event Action RefreshClicked;
        
        public RectTransform CardsParent => listContainer;
        
        private void Awake()
        {
            refreshButton.onClick.AddListener(() => RefreshClicked?.Invoke());
        }
    }
}