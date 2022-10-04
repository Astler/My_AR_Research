using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        private const float ScreenFadeDuration = 1f;

        [SerializeField] private bool fadeInOnStart = true;

        public static Action<float> FadingSceneIn;
        public static Action<float> FadingSceneOut;

        private CanvasGroup _sceneFadeCanvasGroup;
        private Image _backgroundImage;

        private void Awake()
        {
            _backgroundImage = GetComponent<Image>();
            _sceneFadeCanvasGroup = GetComponentInChildren<CanvasGroup>();

            if (_sceneFadeCanvasGroup == null)
            {
                Debug.LogError(name + " didnt't find CanvasGroup in children.");
                return;
            }

            _sceneFadeCanvasGroup.alpha = 0f;
            _backgroundImage.DOFade(0f, 0f);
        }

        public void FadeOutToColor(Color color)
        {
            _backgroundImage.color = color;

            gameObject.SetActive(true);
            _backgroundImage.DOFade(0f, 0f);
            _backgroundImage.DOFade(1f, ScreenFadeDuration);
        }

        public void FadeInFromColor(Color color)
        {
            _backgroundImage.color = color;

            _backgroundImage.DOFade(1f, 0f);
            _backgroundImage.DOFade(0f, ScreenFadeDuration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}