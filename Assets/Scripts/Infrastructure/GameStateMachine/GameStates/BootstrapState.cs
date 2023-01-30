using System;
using System.Reflection;
using Assets;
using Core;
using Core.WebSockets;
using Data;
using DG.Tweening;
using SceneManagement;
using Screens;
using UnityEngine;
using Utils.StateMachine;
using static Data.PlayerPrefsHelper;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class BootstrapState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IApiInterface _apiInterface;
        private readonly IWebSocketService _webSocketService;
        private readonly IDataProxy _dataProxy;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly EditorAssets _editorAssets;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader,
            IApiInterface apiInterface, WebSocketService webSocketService, IDataProxy dataProxy,
            IScreenNavigationSystem screenNavigationSystem, EditorAssets editorAssets)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _apiInterface = apiInterface;
            _webSocketService = webSocketService;
            _dataProxy = dataProxy;
            _screenNavigationSystem = screenNavigationSystem;
            _editorAssets = editorAssets;
        }

        public void Enter()
        {
            _sceneLoader.Load(SceneName.BootScene, OnBootSceneLoaded);
        }

        private void OnBootSceneLoaded()
        {
#if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
#endif
            Debug.Log("Loaded BootScene");

            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand()
                .ShowNextScreen(ScreenName.LoadingScreen).WithoutAnimation());

            if (Application.isEditor && _editorAssets.mockStartData)
            {
                _dataProxy.LoadClaimedRewards();
                _dataProxy.AddEvents(new EventsData
                {
                    events = new[]
                    {
                        new EventData
                        {
                            title = "Editor Mock Zone",
                            latitude = 0,
                            longitude = 0,
                            radius = 20,
                            prizes = new[]
                            {
                                new PrizeData()
                                {
                                    amount = 10,
                                    event_id = 0,
                                    id = 10,
                                    image = "dog.pes",
                                    is_claimed = false,
                                    prize_type = 0
                                }
                            }
                        }
                    }
                });
                _gameStateMachine.Enter<LoadLevelState, SceneName>(SceneName.MainScene);
            }
            else
            {
                _apiInterface.SignIn(
                    delegate(SignInResponse response)
                    {
                        _dataProxy.SetUserData(response.user);
                        AccessToken = response.access_token;
                        _webSocketService.Connect(response.access_token);

                        _apiInterface.GetEventsList(data =>
                        {
                            _dataProxy.LoadClaimedRewards();
                            _dataProxy.AddEvents(data);
                            _gameStateMachine.Enter<LoadLevelState, SceneName>(SceneName.MainScene);
                        }, status => { });
                    }, null);
            }

            Initialize();
        }

        private void Initialize()
        {
            DOTween.Init();
        }

        public void Exit() { }
    }
}