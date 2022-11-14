using System;
using DG.Tweening;
using Screens;
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
        private const float AnimationDuration = 0.25f;

        [SerializeField] protected bool isStartScreen;

        public event Action<IGameStep> Started;
        public event Action<IGameStep> Finished;

        protected CanvasGroup ScreenCanvasGroup =>
            _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        protected RectTransform ScreenTransform =>
            _rectTransform ? _rectTransform : _rectTransform = GetComponent<RectTransform>();
        
        protected Fader SceneFader => _fader;

        private Fader _fader;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Tween _transition;

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

            _transition?.Complete();
            _transition?.Kill();

            if (active)
            {
                ScreenTransform.localScale = Vector3.zero;
                
                gameObject.SetActive(true);
                _transition = DOTween.Sequence(ScreenTransform.DOScale(1f, AnimationDuration))
                    .Join(ScreenCanvasGroup.DOFade(1f, AnimationDuration));
            }
            else
            {
                _transition = DOTween.Sequence(ScreenTransform.DOScale(0f, AnimationDuration))
                    .Join(ScreenCanvasGroup.DOFade(0f, AnimationDuration))
                    .OnComplete(() => gameObject.SetActive(false));
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
    }
}