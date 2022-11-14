using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens;
using Utils.StateMachine;
using Zenject;

namespace Infrastructure.GameStateMachine
{
    public class GameStateMachine : StateMachine, IInitializable
    {
        public GameStateMachine(SceneLoader sceneLoader, IScreenNavigationSystem screenNavigationSystem)
        {
            AddNewState(new BootstrapState(this, sceneLoader));
            AddNewState(new LoadLevelState(this, sceneLoader, screenNavigationSystem));
            AddNewState(new GameLoopState());
        }

        public void Initialize()
        {
            Enter<BootstrapState>();
        }
    }
}