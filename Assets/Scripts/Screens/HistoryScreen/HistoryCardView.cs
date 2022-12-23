using System;
using Data.Objects;
using TMPro;
using UnityEngine;
using Utils;
using Zenject;

namespace Screens.HistoryScreen
{
    public class HistoryCardView : MonoBehaviour, IPoolable<HistoryStepData, IMemoryPool>, IDisposable
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI messageText;

        private IMemoryPool _pool;
        private Transform _transform;

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(HistoryStepData data, IMemoryPool pool)
        {
            _transform.parent = data.Parent;
            
            timeText.text = data.TimeUtc.ConvertToHumanTime();
            messageText.text = data.Message;

            _pool = pool;
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }

        private void Awake()
        {
            _transform = transform;
        }
    }
}