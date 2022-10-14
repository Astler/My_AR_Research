using System;
using Prototype.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens.MainScreen
{
    public interface IMainScreenView
    {
        void SetIsMapActive(bool isMapActive);
        void ConfigureAction(MainSceneHUDViewInfo mainSceneHUDViewInfo);
        void SetCoins(int coins);
        IMapUserInterface GetMapUserInterface();
        void SetCanPlacePortal(bool canPlace);
    }

    public class MainSceneView : MonoBehaviour, IMainScreenView
    {
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button spawnPortalButton;
        [SerializeField] private PlayerBalanceBarView balanceBarView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        [SerializeField] private GameObject allZonesParent;
        [SerializeField] private GameObject portalPlacementParent;
        [SerializeField] private CanvasGroup stepsUserInterfaceCanvasGroup;

        private Action _openMapClicked;
        private Action _clearButtonOnClick;
        private Action _restartButtonOnClick;
        private Action _spawnPortalButton;
        private Action<Vector2> _emptyScreenClicked;

        private void Start()
        {
            clearButton.onClick.AddListener(() => { _clearButtonOnClick?.Invoke(); });
            restartButton.onClick.AddListener(() => { _restartButtonOnClick?.Invoke(); });
            openMapButton.onClick.AddListener(() => { _openMapClicked?.Invoke(); });
            spawnPortalButton.onClick.AddListener(() => { _spawnPortalButton?.Invoke(); });
        }

        public void SetCoins(int coins) => balanceBarView.SetCoins(coins);
        public IMapUserInterface GetMapUserInterface() => mapUserInterfaceView;
        
        public void SetCanPlacePortal(bool canPlace)
        {
            portalPlacementParent.SetActive(canPlace);
        }

        public void ConfigureAction(MainSceneHUDViewInfo viewInfo)
        {
            _clearButtonOnClick = viewInfo.ClearButtonOnClick;
            _restartButtonOnClick = viewInfo.RestartButtonOnClick;
            _spawnPortalButton = viewInfo.SpawnPortalButtonOnClick;
            _emptyScreenClicked = viewInfo.OnScreenClick;
            _openMapClicked = viewInfo.OnOpenMapClick;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !Utils.Utils.IsPointerOverUIObject())
            {
                _emptyScreenClicked?.Invoke(Input.mousePosition);
            }
#else
        if (Input.touches.Length > 0 && !Utils.Utils.IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _emptyScreenClicked?.Invoke(Input.GetTouch(0).position);
            }
        }
#endif
        }

        public void SetIsMapActive(bool isMapActive)
        {
            clearButton.gameObject.SetActive(!isMapActive);
            restartButton.gameObject.SetActive(!isMapActive);
            spawnPortalButton.gameObject.SetActive(!isMapActive);
            allZonesParent.gameObject.SetActive(!isMapActive);
            mapUserInterfaceView.gameObject.SetActive(isMapActive);
            stepsUserInterfaceCanvasGroup.alpha = isMapActive ? 0 : 1;
        }
    }

    public struct MainSceneHUDViewInfo
    {
        public Action ClearButtonOnClick;
        public Action RestartButtonOnClick;
        public Action SpawnPortalButtonOnClick;
        public Action OnOpenMapClick;
        public Action<Vector2> OnScreenClick;
    }
}