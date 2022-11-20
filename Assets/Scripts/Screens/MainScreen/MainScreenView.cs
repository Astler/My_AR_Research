using System;
using DG.Tweening;
using Screens.PortalsListScreen;
using Screens.Views;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private Button closeZonesList;
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button placeRandomBeamButton;
        [SerializeField] private Button warningOkButton;
        [SerializeField] private GameObject placementParent;
        [SerializeField] private LocationInfoView locationInfoView;
        [SerializeField] private PlayerBalanceBarView playerBalancesView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;

        [Space, Header("Steps"), SerializeField]
        private CanvasGroup warningStep;

        [SerializeField] private CanvasGroup scanningStep;
        [SerializeField] private CanvasGroup locationSearchScreen;

        [SerializeField] private GameObject zonesList;
        [SerializeField] private ZonesListView zonesListView;

        public event Action WarningOkClicked;
        public event Action PlaceRandomBeamClicked;
        public event Action OpenMapClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;

        public void SetIsMapActive(bool isMapActive)
        {
            clearButton.gameObject.SetActive(!isMapActive);
            restartButton.gameObject.SetActive(!isMapActive);
            placeRandomBeamButton.gameObject.SetActive(!isMapActive);
            mapUserInterfaceView.gameObject.SetActive(isMapActive);
        }

        public void SetCoins(int coins)
        {
            playerBalancesView.SetCoins(coins);
        }

        public void SetupActiveZone(string zoneName)
        {
            bool hasZone = zoneName != null;

            locationInfoView.SetActiveZoneName(hasZone
                ? $"<color=green>{zoneName}</color>"
                : "<color=red>Go to the portal area!</color>");

            placementParent.SetActive(hasZone);

            if (!hasZone)
            {
                locationInfoView.HideAllZonesList();
            }
        }

        public void ShowLocationSearchStatus(string status) => locationInfoView.ShowResponse(status);

        public MapUserInterfaceView GetMapUserInterface() => mapUserInterfaceView;

        public void HideInterface()
        {
            openMapButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            placeRandomBeamButton.gameObject.SetActive(false);
            locationInfoView.gameObject.SetActive(false);
            playerBalancesView.gameObject.SetActive(false);
            mapUserInterfaceView.gameObject.SetActive(false);

            warningStep.alpha = 0;
            scanningStep.alpha = 0;
            locationSearchScreen.alpha = 0;
            warningStep.interactable = false;
            scanningStep.interactable = false;
            locationSearchScreen.interactable = false;
            warningStep.blocksRaycasts = false;
            scanningStep.blocksRaycasts = false;
            locationSearchScreen.blocksRaycasts = false;
        }

        public void ShowWarningMessage()
        {
            warningStep.DOKill();
            warningStep.interactable = true;
            warningStep.blocksRaycasts = true;
            warningStep.DOFade(1f, 1f);
        }

        public void ShowBaseInterface()
        {
            playerBalancesView.gameObject.SetActive(true);
        }

        public void HideWarningMessage()
        {
            warningStep.DOKill();
            warningStep.interactable = false;
            warningStep.blocksRaycasts = false;
            warningStep.DOFade(0f, 1f);
        }

        public void ShowLocationDetectionPopup()
        {
            locationInfoView.gameObject.SetActive(true);
            locationSearchScreen.DOKill();
            locationSearchScreen.DOFade(1f, 1f);
        }

        public void HideLocationDetectionPopup()
        {
            locationSearchScreen.DOKill();
            locationSearchScreen.DOFade(0f, 1f);
        }

        public void ShowScanningPopup()
        {
            scanningStep.DOKill();
            scanningStep.DOFade(1f, 1f);
        }

        public void HideScanningPopup()
        {
            scanningStep.DOKill();
            scanningStep.DOFade(0f, 1f);
        }

        public void ShowGameInterface()
        {
            openMapButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            placeRandomBeamButton.gameObject.SetActive(true);
            locationInfoView.gameObject.SetActive(true);
            playerBalancesView.gameObject.SetActive(true);
        }

        public void ShowAllZonesList()
        {
            zonesList.SetActive(true);
        }

        public IPortalsListScreenView GetZonesListView() => zonesListView;

        public void HideZonesList()
        {
            zonesList.SetActive(false);
        }

        private void Awake()
        {
            warningOkButton.ActionWithThrottle(() => WarningOkClicked?.Invoke());
            placeRandomBeamButton.ActionWithThrottle(() => PlaceRandomBeamClicked?.Invoke());
            clearButton.ActionWithThrottle(() => { ClearButtonClicked?.Invoke(); });
            restartButton.ActionWithThrottle(() => { RestartButtonClicked?.Invoke(); });
            openMapButton.ActionWithThrottle(() => { OpenMapClicked?.Invoke(); });
            closeZonesList.ActionWithThrottle(HideZonesList);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !Utils.Utils.IsPointerOverUIObject())
            {
                EmptyScreenClicked?.Invoke(Input.mousePosition);
            }
#else
        if (Input.touches.Length > 0 && !Utils.Utils.IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                EmptyScreenClicked?.Invoke(Input.GetTouch(0).position);
            }
        }
#endif
        }
    }
}