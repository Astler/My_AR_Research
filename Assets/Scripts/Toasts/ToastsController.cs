using System;
using System.Collections.Generic;
using System.Linq;
using Screens.Factories;
using UniRx;

namespace Toasts
{
    public interface IToastsController
    {
        void ShowToast(ToastViewInfo viewInfo);
    }

    public class ToastsController : IDisposable, IToastsController
    {
        private readonly ToastsFactory _toastsFactory;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly List<ToastViewInfo> _toastQueue = new();
        private IToastView _activeToast;

        public ToastsController(ToastsFactory toastsFactory)
        {
            _toastsFactory = toastsFactory;
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        public void ShowToast(ToastViewInfo viewInfo)
        {
            if (_activeToast != null)
            {
                if (_toastQueue.Count(it => it.Text == viewInfo.Text) > 0) return;
                
                _toastQueue.Add(viewInfo);
                return;
            }

            viewInfo.Hidden += OnToastHidden;

            _activeToast = _toastsFactory.Create(viewInfo);
        }

        private void OnToastHidden()
        {
            _activeToast?.Dispose();
            _activeToast = null;

            if (_toastQueue.Count > 0)
            {
                ToastViewInfo toastToShow = _toastQueue.First();
                ShowToast(toastToShow);
                _toastQueue.Remove(toastToShow);
            }
        }
    }
}