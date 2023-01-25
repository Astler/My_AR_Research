using System;
using Data.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Screens.FindDropZonesScreen
{
    public class DropZoneCardView : MonoBehaviour, IPoolable<DropZoneViewInfo, IMemoryPool>, IDisposable
    {
        [SerializeField] private TextMeshProUGUI portalName;
        [SerializeField] private TextMeshProUGUI distance;
        [SerializeField] private Button moveToButton;

        private Vector2 _coordinates;
        private IMemoryPool _pool;

        public event Action<Vector2> MoveToClicked;

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(DropZoneViewInfo viewInfo, IMemoryPool pool)
        {
            _pool = pool;
            
            portalName.text = viewInfo.Name;
            distance.text = viewInfo.Distance;
            _coordinates = viewInfo.Coordinates;
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }    
        
        private void Awake()
        {
            moveToButton.onClick.AddListener(() => MoveToClicked?.Invoke(_coordinates));
        }
    }
}