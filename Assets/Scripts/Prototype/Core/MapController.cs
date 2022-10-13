using System.Collections.Generic;
using Prototype.Assets;
using Prototype.World;
using UniRx;
using UnityEngine;

namespace Prototype.Core
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject portalPrefab;
        [SerializeField] private GameObject mapObject;
        [SerializeField] private GameObject mapControlInterface;

        [SerializeField] private OnlineMapsMarker3DManager onlineMapsMarker3DManager;

        private List<OnlineMapsMarker3D> _mapPoints = new();

        // [SerializeField] private MapUserInterfaceView mapObject;

        private ProjectContext _context;

        private void Awake()
        {
            _context = FindObjectOfType<ProjectContext>();
        }

        private void Start()
        {
            _context.MapOpened.Subscribe(delegate(bool active)
            {
                Vector2 playerPosition = LocationController.GetPlayerPosition();
                OnlineMaps.instance.position = playerPosition;

                mapObject.SetActive(active);
                mapControlInterface.SetActive(active);

                foreach (OnlineMapsMarker3D onlineMapsMarker3D in _mapPoints)
                {
                    Destroy(onlineMapsMarker3D.instance);
                }

                onlineMapsMarker3DManager.items.Clear();

                if (!active) return;
                
                _mapPoints.Add(onlineMapsMarker3DManager.Create(playerPosition.x, playerPosition.y, playerPrefab));

                foreach (PortalZoneModel portalZoneModel in _context.GetAssets().portalZones)
                {
                    _mapPoints.Add(onlineMapsMarker3DManager.Create(portalZoneModel.longitude, portalZoneModel.latitude,
                        portalPrefab));
                }
            }).AddTo(this);
        }
    }
}