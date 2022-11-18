using Core;
using Core.WebSockets;
using Data;
using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens;
using Utils.StateMachine;
using Zenject;

namespace Infrastructure.GameStateMachine
{
    public class GameStateMachine : StateMachine, IInitializable
    {
        private readonly IApiInterface _apiInterface;
        private readonly WebSocketService _webSocketService;

        public GameStateMachine(SceneLoader sceneLoader, IScreenNavigationSystem screenNavigationSystem,
            IApiInterface apiInterface, WebSocketService webSocketService, IDataProxy dataProxy)
        {
            _apiInterface = apiInterface;
            _webSocketService = webSocketService;
            AddNewState(new BootstrapState(this, sceneLoader, _apiInterface, _webSocketService, dataProxy,
                screenNavigationSystem));
            AddNewState(new LoadLevelState(this, sceneLoader, screenNavigationSystem));
            AddNewState(new GameLoopState());
        }

        public void Initialize()
        {
            Enter<BootstrapState>();
        }
    }
}