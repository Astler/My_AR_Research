using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens
{
    public class MainSceneView : MonoBehaviour
    {
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button spawnPortalButton;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private TextMeshProUGUI distanceText;

        private Action _clearButtonOnClick;
        private Action _restartButtonOnClick;
        private Action _spawnPortalButton;
        private Action<Vector2> _emptyScreenClicked;

        private void Start()
        {
            clearButton.onClick.AddListener(() => { _clearButtonOnClick?.Invoke(); });
            restartButton.onClick.AddListener(() => { _restartButtonOnClick?.Invoke(); });
            spawnPortalButton.onClick.AddListener(() => { _spawnPortalButton?.Invoke(); });
        }

        public void ConfigureAction(MainSceneHUDViewInfo viewInfo)
        {
            _clearButtonOnClick = viewInfo.ClearButtonOnClick;
            _restartButtonOnClick = viewInfo.RestartButtonOnClick;
            _spawnPortalButton = viewInfo.SpawnPortalButtonOnClick;
            _emptyScreenClicked = viewInfo.OnScreenClick;
        }

        public void ConfigureLocationText(LocationInfo? locationInfo)
        {
            if (locationInfo == null)
            {
                locationText.text = "GPS disabled";
                return;
            }

            LocationInfo nonNullLocationInfo = locationInfo.Value;

            locationText.text = "Location :\nAltitude : " + nonNullLocationInfo.altitude + "\nLatitude : " +
                                nonNullLocationInfo.latitude + "\nLongitude : " + nonNullLocationInfo.longitude;
        }

        public void ConfigureDistanceToTargetText(float distance)
        {
            distanceText.text = "Distance to target: " + distance;
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
    }

    public struct MainSceneHUDViewInfo
    {
        public Action ClearButtonOnClick;
        public Action RestartButtonOnClick;
        public Action SpawnPortalButtonOnClick;
        public Action<Vector2> OnScreenClick;
    }
}