using System;
using JetBrains.Annotations;
using Screens.PortalsListScreen;
using Screens.RewardsListScreen;
using Screens.Views;
using UnityEngine;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        public event Action WarningOkClicked;
        public event Action OpenMapClicked;
        public event Action CollectedRewardsClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;

        void SetNextGiftTime(int timeToNextGift);
        void SetIsMapActive(bool isMapActive);
        void SetCoins(int coins);
        void SetupActiveZone([CanBeNull] string zoneName);
        void ShowLocationSearchStatus(string status);
        IMapUserInterface GetMapUserInterface();
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
        void ShowRewardsList();
        void HideRewardsList();
        IPortalsListScreenView GetZonesListView();
        IRewardsListScreenView GetRewardsListView();
        void HideZonesList();
        void ShowRewardPopup(Sprite sprite, string itemName);
        void ShowAlreadyClaimedRewardPopup(Sprite sprite, string itemName);
        void SetAvailableGifts(int gifts);
    }
}