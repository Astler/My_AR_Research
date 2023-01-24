using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.CollectedRewards
{
    public interface ICollectedRewardsScreenView : IScreenView
    {
        event Action RefreshClicked;
        
        RectTransform GetListContainer();
    }

    public class CollectedRewardsScreenView : ScreenView, ICollectedRewardsScreenView
    {
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button closeButton;

        public event Action RefreshClicked;
        
        public RectTransform GetListContainer() => listContainer;

        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnClose?.Invoke());
            refreshButton.onClick.AddListener(() => RefreshClicked?.Invoke());
        }
    }
}