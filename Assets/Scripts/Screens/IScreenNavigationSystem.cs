namespace Screens
{
    public interface IScreenNavigationSystem
    {
        void ExecuteNavigationCommand(NavigationCommand navigationCommand);
        void HideScreenInformation(IScreenView screenView);
        void ShowScreenInformation(IScreenView screenView);
    }
}