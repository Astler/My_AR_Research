using Screens.Views;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        IMapUserInterface MapUserInterface { get; }
        IBottomNavigationBar BottomNavigationBar { get; }
        
        void SetNextGiftTime(int timeToNextGift);
        void SetAvailableRewards(int rewards);
        void ConfigureView(MainScreenViewInfo getUserInfo);
    }
}