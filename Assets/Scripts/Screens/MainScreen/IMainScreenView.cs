using System;
using JetBrains.Annotations;
using Pointers;
using Screens.Views;
using UnityEngine;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        public event Action OpenMapClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;
        public event Action CollectedRewardsClicked;
        public event Action HistoryClicked;
        
        IDropLocationDirectionPointer DirectionPointer { get; }
        
        void SetNextGiftTime(int timeToNextGift);
        void SetIsMapActive(bool isMapActive);
        void SetCoins(int coins);
        void SetupActiveZone([CanBeNull] string zoneName);
        void ShowLocationSearchStatus(string status);
        IMapUserInterface GetMapUserInterface();
        void HideInterface();
        void ShowLocationInfo();
        void ShowBaseInterface();
        void ShowGameInterface();
        void ShowRewardPopup(Sprite sprite, string itemName);
        void ShowAlreadyClaimedRewardPopup(Sprite sprite, string itemName);
        void SetAvailableGifts(int gifts);
    }
}