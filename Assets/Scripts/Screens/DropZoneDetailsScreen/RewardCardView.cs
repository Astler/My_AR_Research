using System;
using Data.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Screens.DropZoneDetailsScreen
{
    public class RewardCardView : MonoBehaviour, IPoolable<RewardViewInfo, IMemoryPool>, IDisposable
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI availableAmount;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private Button viewButton;

        private IMemoryPool _pool;
        private Transform _transform;
        private Action _viewClicked;

        public void OnSpawned(RewardViewInfo viewInfo, IMemoryPool pool)
        {
            _transform.parent = viewInfo.Parent;
            _transform.SetAsLastSibling();

            availableAmount.text = viewInfo.Count.ToString();
            availableAmount.gameObject.SetActive(viewInfo.Count > 0);

            title.text = viewInfo.Name;
            title.color = viewInfo.IsCollected ? Color.white / 2f : Color.white;

            viewButton.gameObject.SetActive(viewInfo.ViewAction != null);
            _viewClicked = viewInfo.ViewAction;
            
            _pool = pool;
        }

        public void SetRewardIcon(Sprite sprite) => rewardIcon.sprite = sprite;

        public void OnDespawned() => _pool = null;

        public void Dispose() => _pool?.Despawn(this);

        private void Awake()
        {
            _transform = transform;
            viewButton.ActionWithThrottle(() => _viewClicked?.Invoke());
        }
    }
}