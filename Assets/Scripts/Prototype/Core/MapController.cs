using Prototype.Data;
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

        private OnlineMapsMarker3D _playerPointer;

        private ProjectContext _context;

        private void Awake()
        {
            _context = FindObjectOfType<ProjectContext>();
        }

        private void Start()
        {
            _context.GetLocationController().PlayerLocationChanged.Subscribe(CreatePlayerPointer).AddTo(this);

            _context.MapOpened.Subscribe(delegate(bool active)
            {
                Vector2 playerPosition = LocationController.GetPlayerPosition();
                OnlineMaps.instance.position = playerPosition;

                mapObject.SetActive(active);
                mapControlInterface.SetActive(active);

                foreach (OnlineMapsMarker3D onlineMapsMarker3D in onlineMapsMarker3DManager.items)
                {
                    Destroy(onlineMapsMarker3D.instance);
                }

                onlineMapsMarker3DManager.items.Clear();

                if (!active) return;

                CreatePlayerPointer(playerPosition);

                foreach (PortalViewInfo viewInfo in _context.GetAllPortals())
                {
                    onlineMapsMarker3DManager.Create(viewInfo.Coordinates.x, viewInfo.Coordinates.y,
                        portalPrefab);
                }
            }).AddTo(this);
        }

        private void CreatePlayerPointer(Vector2 playerPosition)
        {
            if (_playerPointer != null)
            {
                Destroy(_playerPointer.instance);
                onlineMapsMarker3DManager.items.Remove(_playerPointer);
                _playerPointer = null;
            }

            _playerPointer = onlineMapsMarker3DManager.Create(playerPosition.x, playerPosition.y, playerPrefab);
        }
    }
}