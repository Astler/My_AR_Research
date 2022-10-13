using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens.MainScreen
{
    public interface IMainScreenView
    {
        void SetIsMapActive(bool isMapActive);
        void ConfigureAction(MainSceneHUDViewInfo mainSceneHUDViewInfo);
        void SetCoins(int coins);
    }

    public class MainSceneView : MonoBehaviour, IMainScreenView
    {
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button spawnPortalButton;
        [SerializeField] private PlayerBalanceBarView balanceBarView;

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