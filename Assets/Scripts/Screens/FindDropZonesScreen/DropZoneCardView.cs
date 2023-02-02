using System;
using System.Linq;
using Data.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Screens.FindDropZonesScreen
{
    public class DropZoneCardView : MonoBehaviour, IPoolable<DropZoneViewInfo, IMemoryPool>, IDisposable
    {
        [SerializeField] private TextMeshProUGUI zoneTitle;
        [SerializeField] private TextMeshProUGUI distanceToText;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private TextMeshProUGUI availableDropsText;
        [SerializeField] private Button viewZoneInfo;

        private IMemoryPool _pool;
        private int _id;

        public event Action<int> ViewZoneInfoClicked;

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(DropZoneViewInfo viewInfo, IMemoryPool pool)
        {
            _pool = pool;
            _id = viewInfo.Id;

            zoneTitle.text = viewInfo.Name;
            distanceToText.text = viewInfo.OrderDistance * 1000 < viewInfo.Radius
                ? "^in_zone".GetTranslation()
                : "<b>" + viewInfo.ReadableDistance + "</b>";
            durationText.text = viewInfo.IsOngoing()
                ? "^ongoing".GetTranslation()
                : "^in_time".GetTranslation(TimeSpan.FromSeconds(viewInfo.GetTimeToStart()).ToReadableTimeSpan());
            availableDropsText.text = "^card_drops".GetTranslation(viewInfo.Rewards.Count(it => !it.IsCollected));
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }

        private void Awake()
        {
            viewZoneInfo.onClick.AddListener(() => ViewZoneInfoClicked?.Invoke(_id));
        }
    }
}