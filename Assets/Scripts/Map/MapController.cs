using System.Collections.Generic;
using Data;
using Data.Objects;
using UniRx;
using UnityEngine;
using Zenject;

namespace Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapPlayer playerPrefab;
        [SerializeField] private MapDropZone mapDropZonePrefab;
        [SerializeField] private Map map;
        [SerializeField] private Camera mapCamera;

        [SerializeField] private OnlineMapsMarker3DManager onlineMapsMarker3DManager;

        private OnlineMapsMarker3D _playerPointer;
        private IDataProxy _dataProxy;
        private readonly List<OnlineMapsMarker3D> _dropZoneMarkers = new();

        [Inject]
        public void Construct(IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
        }

        private void Start()
        {
            map.Clicked += OnMapClicked;

            _dataProxy.PlayerLocationChanged.Subscribe(PlacePlayerOnMap).AddTo(this);
            _dataProxy.MapOpened.Subscribe(SetupMap).AddTo(this);
        }

        private void OnMapClicked()
        {
            Debug.Log($"clicked on map");
            _dataProxy.SetSelectedOnMapDropZone(-1);
        }

        private void SetupMap(bool isActive)
        {
            Vector2 playerPosition = _dataProxy.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(playerPosition.y, playerPosition.x);

            mapCamera.gameObject.SetActive(isActive);

            foreach (OnlineMapsMarker3D onlineMapsMarker3D in _dropZoneMarkers)
            {
                onlineMapsMarker3DManager.items.Remove(onlineMapsMarker3D);
                Destroy(onlineMapsMarker3D.instance);
            }

            if (!isActive) return;

            PlacePlayerOnMap(playerPosition);

            foreach (DropZoneViewInfo viewInfo in _dataProxy.GetAllActiveZones())
            {
                OnlineMapsMarker3D mapItem = onlineMapsMarker3DManager.Create(viewInfo.Coordinates.y,
                    viewInfo.Coordinates.x, mapDropZonePrefab.gameObject);

                mapItem.scale = viewInfo.Radius;

                _dropZoneMarkers.Add(mapItem);

                MapDropZone dropZone = mapItem.instance.GetComponent<MapDropZone>();

                if (dropZone == null) continue;

                int id = viewInfo.Id;

                _dataProxy.SelectedOnMapDropZoneId
                    .Subscribe(selectedId => { dropZone.SetIsSelected(selectedId == id); }).AddTo(dropZone);

                dropZone.Clicked += delegate { OnDropZoneClicked(id); };
            }
        }

        private void PlacePlayerOnMap(Vector2 playerPosition)
        {
            if (_playerPointer == null)
            {
                _playerPointer =
                    onlineMapsMarker3DManager.Create(playerPosition.y, playerPosition.x, playerPrefab.gameObject);

                MapPlayer mapPlayer = _playerPointer.instance.GetComponent<MapPlayer>();
                mapPlayer.Clicked += OnPlayerClicked;
            }

            _playerPointer.SetPosition(playerPosition.y, playerPosition.x);
        }

        private void OnDropZoneClicked(int id)
        {
            Debug.Log($"clicked {id}");
            _dataProxy.SetSelectedOnMapDropZone(id);
        }

        private void OnPlayerClicked()
        {
            Debug.Log("clicked on player");
        }
    }
}