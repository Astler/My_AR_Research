using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Toasts
{
    public interface IToastView : IDisposable { }

    public class ToastView : MonoBehaviour, IToastView, IPoolable<ToastViewInfo, IMemoryPool>
    {
        [SerializeField] private TextMeshProUGUI toastText;

        private RectTransform _transform;
        private IMemoryPool _pool;

        private void Awake()
        {
            _transform = (RectTransform)transform;
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(ToastViewInfo viewInfo, IMemoryPool pool)
        {
            _pool = pool;
            toastText.text = viewInfo.Text;

            viewInfo.Showed?.Invoke();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);

            _transform.anchoredPosition = new Vector2(_transform.anchoredPosition.x, 0f);

            StartCoroutine(DelayerTween(viewInfo));
        }

        private IEnumerator DelayerTween(ToastViewInfo viewInfo)
        {
            yield return new WaitForSeconds(0.1f);
            
            DOTween.Sequence().Append(_transform.DOAnchorPosY(-_transform.sizeDelta.y, 0.5f))
                .AppendInterval(viewInfo.Duration).Append(_transform.DOAnchorPosY(0, 0.5f))
                .OnComplete(() => { viewInfo.Hidden?.Invoke(); });
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }
    }
}