using JetBrains.Annotations;
using Screens.Views;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        IMapUserInterface MapUserInterface { get; }
        IBottomNavigationBar BottomNavigationBar { get; }
        
        void SetUIFlags(MainScreenMode mainScreenUI);
        void ShowLocationSearchStatus(string status);
        void SetNextGiftTime(int timeToNextGift);
        void SetupActiveZone([CanBeNull] string zoneName);
        void SetAvailableRewards(int rewards);
    }
}