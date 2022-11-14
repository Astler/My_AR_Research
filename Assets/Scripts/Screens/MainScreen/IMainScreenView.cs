using System;
using JetBrains.Annotations;
using Screens.PortalsListScreen;
using Screens.Views;
using UnityEngine;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        public event Action WarningOkClicked;
        public event Action PlaceRandomBeamClicked;
        public event Action OpenMapClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;

        void SetIsMapActive(bool isMapActive);
        void SetCoins(int coins);
        void SetupActiveZone([CanBeNull] string zoneName);
        void ShowLocationSearchStatus(string status);
        MapUserInterfaceView GetMapUserInterface();
        void HideInterface();
        void ShowWarningMessage();
        void ShowBaseInterface();
        void HideWarningMessage();
        void ShowLocationDetectionPopup();
        void HideLocationDetectionPopup();
        void ShowScanningPopup();
        void HideScanningPopup();
        void ShowGameInterface();
        void ShowAllZonesList();
        IPortalsListScreenView GetZonesListView();
    }
}