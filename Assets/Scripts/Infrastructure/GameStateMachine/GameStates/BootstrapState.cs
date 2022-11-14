using System;
using System.Reflection;
using DG.Tweening;
using SceneManagement;
using UnityEngine;
using Utils.StateMachine;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class BootstrapState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
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