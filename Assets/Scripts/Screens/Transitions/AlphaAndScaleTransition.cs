using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Screens.Transitions
{
    public class AlphaAndScaleTransition : IScreenTransitionAnimation
    {
        private readonly CanvasGroup _canvasGroup;
        private readonly Transform _transform;
        private readonly float _duration;
        private readonly Vector3 _fromScale;
        private readonly Vector3 _toScale;
        private readonly float _fromAlpha;
        private readonly float _toAlpha;
        
        private TweenerCore<float, float, FloatOptions> _anim;
        private bool _isPlaying;
        
        public AlphaAndScaleTransition(CanvasGroup canvasGroup, float fromAlpha, float toAlpha,
            float fromScale, float toScale, float showDuration = 0.25f)
        {
            _canvasGroup = canvasGroup;
            _transform = canvasGroup.transform;
            _duration = showDuration;
            _fromScale = Vector3.one * fromScale;
            _toScale = Vector3.one * toScale;
            _fromAlpha = fromAlpha;
            _toAlpha = toAlpha;
        }

        public void PerformAnimation(Action onEndCallback = null)
        {
            _isPlaying = true;
            _canvasGroup.alpha = _fromAlpha;
            _anim = _canvasGroup.DOFade(_toAlpha, _duration).OnComplete(delegate
            {
                _isPlaying = false;
                onEndCallback?.Invoke();
            });
            
            _transform.localScale = _toScale;
            if (_fromScale != _toScale)
            {
                _transform.DOScale(Vector3.one, _duration).SetEase(Ease.OutCubic);
            }
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public void KillAnim()
        {
            if (_anim == null) return;
            _anim.Kill();
            _anim = null;
            _canvasGroup.alpha = _toAlpha;
            _transform.localScale = _toScale;
        }
    }
}
