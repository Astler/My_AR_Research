using System;
using System.Collections;
using Screens;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneLoader
    {
        private static ReactiveProperty<float> _loadSceneProgress = new ReactiveProperty<float>(0);
        public IReadOnlyReactiveProperty<float> LoadSceneProgress => _loadSceneProgress;

        public void Load(SceneName name, Action onLoaded = null)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            Debug.Log("Try to load scene: " + name);
            _loadSceneProgress.Value = 0f;
            MainThreadDispatcher.StartCoroutine(LoadScene(name, onLoaded));
        }

        private static IEnumerator LoadScene(SceneName nextScene, Action onLoaded)
        {
            string activeSceneName = SceneManager.GetActiveScene().name;
            Debug.Log("activeSceneName: " + activeSceneName + " | nextScene: " + nextScene.ToString());
            if (activeSceneName == nextScene.ToString())
            {
                onLoaded?.Invoke();
                yield break;
            }
            
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene.ToString());
            
            waitNextScene.completed += delegate(AsyncOperation operation)
            {
                onLoaded?.Invoke();
            };
  
            while (true)
            {
                activeSceneName = SceneManager.GetActiveScene().name;
                if (activeSceneName == nextScene.ToString())
                {
                    _loadSceneProgress.Value = 1f;
                    yield break;
                }
                Debug.Log("_loadSceneProgress: " + _loadSceneProgress.Value);
                float realProgress = Mathf.Clamp(waitNextScene.progress-0.1f,0f,1f);
                if (realProgress > _loadSceneProgress.Value) _loadSceneProgress.Value = realProgress;
                yield return null;
            }
        }
    }
}