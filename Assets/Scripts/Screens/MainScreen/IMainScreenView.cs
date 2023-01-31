using System;
using Screens.Views;

namespace Screens.MainScreen
{
    public interface IMainScreenView : IScreenView
    {
        event Action MenuClicked;
        IMapUserInterface MapUserInterface { get; }
        IBottomNavigationBar BottomNavigationBar { get; }

        void ConfigureView(MainScreenViewInfo getUserInfo);
    }
}