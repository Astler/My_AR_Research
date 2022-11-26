using System;
using System.Collections;
using DG.Tweening;
using Plugins.Honeti.I18N.Scripts;
using Screens.Transitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Screens.LoadingScreen
{
    public class LoadingScreenView : ScreenView, ILoadingScreenView
    {
        [SerializeField] private TextMeshProUGUI playerIdText;
        [SerializeField] private TextMeshProUGUI appVersionText;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Slider loadingProgress;
        public event Action OnLoadingAnimationFinish;

        private const string _Loading = "^loading";
        private const string Version = "^version";

        private Coroutine _loadingCoroutine;
        private float _currentProgress;

        private void Awake()
        {
            SetShowTransitionAnimation(new AlphaAndScaleTransition(CanvasGroup, 0f, 1f, 1f, 1f, 0.5f));
            SetHideTransitionAnimation(new AlphaAndScaleTransition(CanvasGroup, 1f, 0f, 1f, 1f, 0.5f));
        }

        public void SetViewModel(string appVersion)
        {
            Debug.Log(I18N.instance.GetValue(Version));
            appVersionText.text = $"{I18N.instance.GetValue(Version)}: {appVersion}";
            StartCoroutine(BlinkText(0.3f));
        }

        public void SetLoadingProgressValue(float progress)
        {
            if (progress == 0)
            {
                loadingProgress.value = 0f;
            }
            
            Debug.Log("Progress: " + progress);
            _currentProgress = progress;
            _loadingCoroutine ??= StartCoroutine(Loading());
        }

        private IEnumerator BlinkText(float duration)
        {
            Transform textTransform = loadingText.transform;
            Vector3 originalScale = textTransform.localScale;
            float currentAlpha = loadingText.color.a;
            while (true)
            {
                int delta = currentAlpha >= 1f ? -1 : 1;
                float targetAlpha = 0.75f + delta * 0.25f;
                while ((delta < 0 && currentAlpha > targetAlpha) || (delta > 0 && currentAlpha < targetAlpha))
                {
                    currentAlpha += Time.deltaTime * duration * delta;
                    loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b,
                        currentAlpha);
                    textTransform.localScale = originalScale * Mathf.Lerp(0.8f, 1f, currentAlpha);
                    yield return null;
                }
            }
        }

        private IEnumerator Loading()
        {
            float progress = _currentProgress;
            while (true)
            {
                progress = Mathf.Clamp(progress + Random.Range(0.08f, 0.15f), 0f, _currentProgress);
                float stepPause = Random.Range(0.2f, 0.4f);
                loadingProgress.DOValue(progress, stepPause);
                loadingText.text = $"{I18N.instance.GetValue(_Loading)} ({(int)(progress * 100)}%)";
                yield return new WaitForSeconds(stepPause);
                if (progress > 0.99f)
                {
                    StopCoroutine(_loadingCoroutine);
                    _loadingCoroutine = null;
                    
                    OnLoadingAnimationFinish?.Invoke();
                }
            }
        }
    }
}