using System;
using JetBrains.Annotations;
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

        IMapUserInterface MapUserInterface { get; }
        
        void SetUIFlags(MainScreenMode mainScreenUI);

        void ShowLocationSearchStatus(string status);
        void SetNextGiftTime(int timeToNextGift);
        void SetCoins(int coins);
        void SetupActiveZone([CanBeNull] string zoneName);
        void SetAvailableRewards(int rewards);
    }
}