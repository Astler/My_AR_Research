using System;
using DG.Tweening;
using UnityEngine;

namespace Screens.Transitions
{
    public class RightToFrontAnimation : IScreenTransitionAnimation
    {
        private readonly RectTransform _rectTransform;
        private readonly CanvasGroup _canvasGroup;
        private readonly RectTransform _safeAreaRectTransform;
        private readonly bool _reverse;
        private readonly float _duration = 0.3f;

        private bool _isPlaying;
        private float _animTime;
        private Vector2 _oldPos;
        private Vector2 _toPos;
        private Vector3 _previousPos;
        private Tween _anim;

        public RightToFrontAnimation(CanvasGroup canvasGroup, bool reverse)
        {
            _safeAreaRectTransform = canvasGroup.transform.parent.GetComponent<RectTransform>();
            _canvasGroup = canvasGroup;
            _rectTransform = canvasGroup.gameObject.GetComponent<RectTransform>();
            _reverse = reverse;
        }

        public void PerformAnimation(Action onEndCallback = null)
        {
            RecalculateStartAndEndPositions();
            _rectTransform.position = _oldPos;

            _canvasGroup.alpha = 1;
            _isPlaying = true;

            _anim = _rectTransform.DOMove(_toPos, _duration).OnComplete(delegate
            {
                _isPlaying = false;
                onEndCallback?.Invoke();
            });
        }

        private void RecalculateStartAndEndPositions()
        {
            Vector3 position = _safeAreaRectTransform.position;
            _toPos.y = _oldPos.y = position.y;
            _oldPos.x = position.x * (_reverse ? 1 : 3);
            _toPos.x = position.x * (_reverse ? 3 : 1);
        }

        public bool IsPlaying() => _isPlaying;

        public void KillAnim()
        {
            if (_anim == null) return;
            _anim.Kill();
            _anim = null;
            _rectTransform.position = _toPos;
            _canvasGroup.alpha = 1;
            _isPlaying = false;
        }
    }
}