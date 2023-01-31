using System;
using System.Collections;
using System.Collections.Generic;
using Screens.Transitions;
using UnityEngine;

namespace Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(UILayerConfigurator))]
    public class ScreenView : MonoBehaviour, IScreenView
    {
        [SerializeField] private bool isTab;
        [Space, SerializeField] private bool shouldPutInNavStack = true;
        [SerializeField] private bool shouldDeleteAfterHide = false;
        [SerializeField] private List<CanvasGroup> infoBlocks = new ();
        
        public event Action<object> OnShowCallback;
        public event Action OnGotFocusCallback;
        public event Action OnLostFocusCallback;
        public event Action OnScreenActivated;
        public event Action ClosedScreen;
        
        public Action OnHideCallback;
        public Action OnClose;
        public IScreenTransitionAnimation OnShowTransitionAnimation;
        public IScreenTransitionAnimation OnHideTransitionAnimation;

        private bool _isOnPosition;
        private float _inactiveTimer;
        private CanvasGroup _canvasGroup;
        private bool _isFocused;
        private UILayerConfigurator _uiLayerConfigurator;
        private IEnumerator _turnOffProcess;
        
        public List<CanvasGroup> InfoBlocks => infoBlocks;
        public bool IsFocused => _isFocused;
        public bool IsRootScreen => UILayerConfigurator.GetOrderLayer() == UIOrderLayer.Root;
        public CanvasGroup CanvasGroup => _canvasGroup ??= GetComponent<CanvasGroup>();
        public UILayerConfigurator UILayerConfigurator => _uiLayerConfigurator ??= GetComponent<UILayerConfigurator>();
        public bool ShouldPutInNavStack => shouldPutInNavStack;
        public bool ShouldDeleteAfterHide => shouldDeleteAfterHide;
        public bool IsTab => isTab;

        public void PrepareScreenView()
        {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
            CanvasGroup.alpha = 0;
        }

        public void ShowOnPosition(object extraData)
        {
            ActivateScreen();
            _isOnPosition = true;
            OnShowTransitionAnimation?.KillAnim();
            InvokeShowWith(extraData);
            GotFocus();
        }

        private void ActivateScreen()
        {
            Debug.Log("ScreenActivate: " + name);
            if (_turnOffProcess != null) StopCoroutine(_turnOffProcess);
            gameObject.SetActive(true);
            OnScreenActivated?.Invoke();
        }

        private void DeactivateScreen()
        {
            Debug.Log("ScreenDeactivate: " + name);
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            LostFocus();
        }

        public void MoveToInitialPosition()
        {
            _isOnPosition = false;
            OnHideCallback?.Invoke();
            DeactivateScreen();
        }

        public void GotFocus()
        {
            if (_isFocused) return;
            _isFocused = true;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.alpha = 1;
            UILayerConfigurator.BackToDefaultOrder();
            Debug.Log("ScreenGotFocus: " + name);
            OnGotFocusCallback?.Invoke();
        }

        public void LostFocus()
        {
            if (!_isFocused) return;
            _isFocused = false;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            Debug.Log("ScreenLostFocus: " + name);
            OnLostFocusCallback?.Invoke();
        }

        public void InactivateScreenWithDelay(float delay)
        {
            _turnOffProcess = ScreenTurnOff(delay);
            StartCoroutine(_turnOffProcess);
        }
        
        private IEnumerator ScreenTurnOff(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_isFocused) yield break;
            UILayerConfigurator.SetLowOrder();
            gameObject.SetActive(false);
        }

        public void PerformHideAnimation(Action callback)
        {
            OnShowTransitionAnimation?.KillAnim();
            UILayerConfigurator.SetHideAnimatingOrder();
            OnHideTransitionAnimation?.PerformAnimation(delegate
            {
                ClosedScreen?.Invoke();
                callback?.Invoke();
            });
        }

        public void PerformShowAnimationWhenReady(Action callback)
        {
            ActivateScreen();
            UILayerConfigurator.SetShowAnimatingOrder();
            OnHideTransitionAnimation?.KillAnim();
            OnShowTransitionAnimation?.PerformAnimation(delegate
            {
                GotFocus();
                callback();
            });
        }

        public void InvokeShowWith(object extraData)
        {
            OnShowCallback?.Invoke(extraData);
        }

        public void SetShowTransitionAnimation(IScreenTransitionAnimation animation)
        {
            OnShowTransitionAnimation = animation;
        }

        public void SetHideTransitionAnimation(IScreenTransitionAnimation animation)
        {
            OnHideTransitionAnimation = animation;
        }

        public void CloseScreen()
        {
            OnClose();
        }

        public void LayUnderScreen(int shift = 1)
        {
            UILayerConfigurator.SetDefaultLayer(shift);
        }
    }
}