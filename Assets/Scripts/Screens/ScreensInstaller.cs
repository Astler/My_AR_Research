using System;
using System.Collections.Generic;
using Assets;
using Data;
using GameCamera;
using SceneManagement;
using Screens.LoadingScreen;
using Screens.MainScreen;
using UnityEngine;
using Zenject;

namespace Screens
{
    public class ScreensInstaller : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> _localScreens = new ();
        
        private SceneLoader _sceneLoader;
        private IScreenNavigationSystem _screenNavigationSystem;
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(ScreenAssets screenAssets, SceneLoader sceneLoader, IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
            _sceneLoader = sceneLoader;
            _screenNavigationSystem = screenNavigationSystem;
            foreach (var somePrefab in screenAssets.screenPrefabs)
            {
                _localScreens.Add(somePrefab.name, somePrefab);
            }
        }
        
        private void InstantiateView<T>(string screenName, Action<T> onSuccess)
        {
            GameObject newScreen;
     
            if (_localScreens.ContainsKey(screenName))
            {
                newScreen = Instantiate(_localScreens[screenName], transform);
                SetupScreen(false);
            }
            else
            {
                Debug.LogWarning($"Screen {screenName} not found in local list. Try load from Addressables.");
            }

            void SetupScreen(bool fromAddressable)
            {
                newScreen.name = screenName;
                if (newScreen.TryGetComponent(out T view))
                {
                    onSuccess.Invoke(view);
                    ScreenView screenView = newScreen.GetComponent<ScreenView>();
                    if (screenView.ShouldDeleteAfterHide)
                    {
                        screenView.OnHideCallback += () =>
                        {
                            
                        };
                    }
                }
                else
                {
                    Debug.LogError("Script not found on instantiated screen.");
                }
            }
        }
        
        public void AddScreenToScene(ScreenName name, Action<ScreenView> onSuccess)
        {
            switch (name)
            {
                case ScreenName.LoadingScreen:
                    InstantiateView(name.ToString(), delegate(LoadingScreenView loadingScreenView)
                    {
                        new LoadingScreenPresenter(loadingScreenView, _sceneLoader, _screenNavigationSystem,
                            _dataProxy);
                        onSuccess.Invoke(loadingScreenView);
                    });
                    break;
                case ScreenName.MainScreen:
                    InstantiateView(name.ToString(), delegate(MainScreenView mainScreenView)
                    {
                        new MainScreenPresenter(mainScreenView, _screenNavigationSystem, _dataProxy);
                        onSuccess.Invoke(mainScreenView);
                    });
                    break;
                    break;
                default:
                    Debug.LogError($"Screen {name} not found in ScreenInstaller");
                    break;
            }
            
        }
    }
}