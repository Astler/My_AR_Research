using Data;
using Data.Objects;
using Geo;
using UniRx;
using UnityEngine;
using Zenject;

namespace AR.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject portalPrefab;
        [SerializeField] private GameObject mapObject;

        [SerializeField] private OnlineMapsMarker3DManager onlineMapsMarker3DManager;

        private OnlineMapsMarker3D _playerPointer;

        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
        }

        private void Start()
        {
            _dataProxy.PlayerLocationChanged.Subscribe(CreatePlayerPointer).AddTo(this);

            _dataProxy.MapOpened.Subscribe(delegate(bool active)
            {
                Vector2 playerPosition = _dataProxy.GetPlayerPosition();
                OnlineMaps.instance.position = new Vector2(playerPosition.y, playerPosition.x);

                mapObject.SetActive(active);

                foreach (OnlineMapsMarker3D onlineMapsMarker3D in onlineMapsMarker3DManager.items)
                {
                    Destroy(onlineMapsMarker3D.instance);
                }

                onlineMapsMarker3DManager.items.Clear();

                if (!active) return;

                CreatePlayerPointer(playerPosition);

                foreach (ZoneViewInfo viewInfo in _dataProxy.GetAllActiveZones())
                {
                    onlineMapsMarker3DManager.Create(viewInfo.Coordinates.y, viewInfo.Coordinates.x,
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

            _playerPointer = onlineMapsMarker3DManager.Create(playerPosition.y, playerPosition.x, playerPrefab);
        }
    }
}