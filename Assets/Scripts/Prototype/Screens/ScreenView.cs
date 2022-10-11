using System;
using DG.Tweening;
using UnityEngine;

namespace Prototype.Screens
{
    public interface IGameStep
    {
        event Action<IGameStep> Started;
        event Action<IGameStep> Finished;
        void StartStep();
        void FinishStep();
        void SetActive(bool active, bool isActive);
    }

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenView : MonoBehaviour, IGameStep
    {
        private const float AnimationDuration = 1f;

        [SerializeField] protected bool isStartScreen;

        public event Action<IGameStep> Started;
        public event Action<IGameStep> Finished;

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

        public void StartStep()
        {
            SetActive(true);
            OnStepStarted();
            Started?.Invoke(this);
        }

        public void FinishStep()
        {
            OnStepFinished();
            SetActive(false);
            Finished?.Invoke(this);
        }

        protected virtual void OnStepStarted()
        {
        }

        protected virtual void OnStepFinished()
        {
        }

        public void SetActive(bool active, bool instant = false)
        {
            if (instant)
            {
                gameObject.SetActive(active);
                return;
            }

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
    }
}