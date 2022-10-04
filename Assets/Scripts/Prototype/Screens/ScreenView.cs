using DG.Tweening;
using UnityEngine;

namespace Prototype.Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : MonoBehaviour
    {
        private const float AnimationDuration = 1f;

        [SerializeField] protected bool isStartScreen;

        protected CanvasGroup ScreenCanvasGroup =>
            _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        protected RectTransform ScreenTransform =>
            _rectTransform ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        protected SceneFinder SceneFinder => SceneFinder.Instance;

        protected Fader SceneFader => _fader;
        protected BackdropView BackdropView => _backdropView;

        private Fader _fader;
        private BackdropView _backdropView;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        private void Awake()
        {
            _fader = SceneFinder.TryGet<Fader>();
            _backdropView = SceneFinder.TryGet<BackdropView>();
            gameObject.SetActive(isStartScreen);

            OnAwake();
        }

        private void Start()
        {
            if (isStartScreen)
            {
                SceneFader.FadeInFromColor(Color.black);
            }

            OnStart();
        }

        private void FadeOutContent()
        {
            ScreenCanvasGroup.DOFade(0f, AnimationDuration)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void FadeInContent()
        {
            gameObject.SetActive(true);
            ScreenCanvasGroup.DOFade(1f, AnimationDuration);
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                ScreenTransform.DOScale(1f, AnimationDuration);
                FadeInContent();
            }
            else
            {
                ScreenTransform.DOScale(0f, AnimationDuration);
                FadeOutContent();
            }
        }
    }
}