using System;
using System.Reflection;
using Core;
using Core.WebSockets;
using Data;
using DG.Tweening;
using SceneManagement;
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
        private readonly WebSocketService _webSocketService;
        private readonly IDataProxy _dataProxy;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader,
            IApiInterface apiInterface, WebSocketService webSocketService, IDataProxy dataProxy)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _apiInterface = apiInterface;
            _webSocketService = webSocketService;
            _dataProxy = dataProxy;
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
            
            _apiInterface.SignIn(
                delegate(SignInResponse response)
                {
                    AccessToken = response.access_token;
                    _webSocketService.Connect(response.access_token);

                    _apiInterface.GetEventsList(data =>
                    {
                        _dataProxy.AddEvents(data);
                    }, status => { });
                }, null);
            
            Initialize();
            
            _gameStateMachine.Enter<LoadLevelState, SceneName>(SceneName.MainScene);
        }

        private void Initialize()
        {
            DOTween.Init();
        }

        public void Exit() { }
    }
}