using System;

namespace Screens.MainScreen
{
    [Flags]
    public enum MainScreenMode
    {
        Hide = 1,
        TopBar = 2,
        MidContent = 4,
        BottomBar = 8,
        Map = 16,
    }
}