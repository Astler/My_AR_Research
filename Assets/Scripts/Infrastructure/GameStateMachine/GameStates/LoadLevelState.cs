using System;
using SceneManagement;
using Screens;
using UniRx;
using UnityEngine;
using Utils.StateMachine;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class LoadLevelState : IPayloadedState<SceneName>
    {
        private const int SceneLoadDelay = 1;
        private readonly SceneLoader _sceneLoader;
        private readonly GameStateMachine _gameStateMachine;
        private readonly IScreenNavigationSystem _screenNavigationSystem;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, IScreenNavigationSystem screenNavigationSystem)
        {
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
            _screenNavigationSystem = screenNavigationSystem;
        }

        public void Enter(SceneName sceneName)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.LoadingScreen).WithoutAnimation());
            Observable.Timer(TimeSpan.FromSeconds(SceneLoadDelay)).Subscribe(delegate(long l)
            {
                _sceneLoader.Load(sceneName, delegate { OnSceneLoaded(sceneName); });
            });
        }

        private void OnSceneLoaded(SceneName sceneName)
        {
            Debug.Log("Loaded " + sceneName);
            _gameStateMachine.Enter<GameLoopState>();
        }

        public void Exit() { }
    }
}