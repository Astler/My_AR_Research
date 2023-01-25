using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Screens
{
    public class ScreenNavigationSystem : IScreenNavigationSystem
    {
        private readonly List<ScreenView> _navigationStack = new();

        private readonly Dictionary<ScreenName, ScreenView> _availableScreens = new();

        private readonly Queue<NavigationCommand> _nextScreenInfos = new();
        private readonly Queue<NextScreenInfo> _waitingScreenInfos = new();

        private ScreenView _currentShowAnimatedScreenView;
        private IEnumerator _showInformationProcess;
        private readonly ScreensInstaller _screensInstaller;

        public ScreenNavigationSystem(ScreensInstaller screensInstaller)
        {
            _screensInstaller = screensInstaller;
        }

        private void GetNewScreen(ScreenName name, Action onSuccess)
        {
            _screensInstaller.AddScreenToScene(name, screen =>
            {
                AddScreen(name, screen);
                onSuccess.Invoke();
            });
        }

        private void AddScreen(ScreenName name, ScreenView screenView)
        {
            _availableScreens.Add(name, screenView);
            screenView.PrepareScreenView();
            screenView.OnClose = delegate { Close(screenView); };
            if (screenView.ShouldDeleteAfterHide) screenView.OnHideCallback += () => _availableScreens.Remove(name);
            AddScreenToStack(screenView);
            screenView.ShowOnPosition(null);
        }

        public void ExecuteNavigationCommand(NavigationCommand navigationCommand)
        {
            if (navigationCommand.IsDelayed && _currentShowAnimatedScreenView != null)
            {
                _nextScreenInfos.Enqueue(navigationCommand);
                return;
            }

            navigationCommand.IsDelayed = false;

            if (_navigationStack.Count != 0)
            {
                if (navigationCommand.IsNextScreenInQueue())
                {
                    PreparePreviousScreens(navigationCommand);
                }

                if (navigationCommand.IsCloseCurrentScreen)
                {
                    Close(_navigationStack.Last(), navigationCommand.IsNextScreenInQueue());
                }

                if (navigationCommand.ScreenToClose != null)
                {
                    Close(navigationCommand.ScreenToClose, navigationCommand.IsNextScreenInQueue());
                }

                if (navigationCommand.IsCloseAllScreens)
                {
                    ForceCloseAllScreens();
                }

                if (navigationCommand.ShouldCloseOtherTabs)
                {
                    foreach (ScreenView screenView in _navigationStack.ToList())
                    {
                        if (screenView.IsTab)
                        {
                            screenView.CloseScreen();
                        }
                    }
                }
            }

            if (!navigationCommand.IsNextScreenInQueue()) return;
            var isReturnedToPrevious = false;
            if (navigationCommand.CanReturnToPreviousScreen)
            {
                isReturnedToPrevious = ReturnToPreviousScreenInStack(navigationCommand);
            }

            if (isReturnedToPrevious) return;
            Show(navigationCommand.NextScreenName, navigationCommand.ExtraData,
                navigationCommand.IsWithAnimation,
                navigationCommand.IsDelayed);
        }

        private void PreparePreviousScreens(NavigationCommand navigationCommand)
        {
            if (!_availableScreens.ContainsKey(navigationCommand.NextScreenName))
            {
                //GetNewScreen(navigationCommand.NextScreenName, () => PreparePreviousScreens(navigationCommand));
                return;
            }

            var nextScreen = _availableScreens[navigationCommand.NextScreenName];

            //lay stack under next screen layer to correct show animation
            LayStackUnderNextScreen(nextScreen);

            var peek = _navigationStack.Last();
            if (peek == nextScreen) return;
            peek.LostFocus();

            void OnNewScreenActivatedAction()
            {
                if (navigationCommand.ShouldCloseAfterNextScreenShown)
                {
                    Close(peek);
                }

                nextScreen.OnScreenActivated -= OnNewScreenActivatedAction;
            }

            nextScreen.OnScreenActivated += OnNewScreenActivatedAction;
        }

        private void LayStackUnderNextScreen(ScreenView nextScreen)
        {
            if (nextScreen == null) return;
            var array = _navigationStack.ToArray();
            for (var index = 0; index < array.Length; index++)
            {
                ScreenView screenView = array[index];
                screenView.LayUnderScreen(index + 1);
            }

            nextScreen.LayUnderScreen(array.Length + 1);
        }

        private bool ReturnToPreviousScreenInStack(NavigationCommand navigationCommand)
        {
            bool isReturnToPrevious = false;
            var nextScreen = _availableScreens[navigationCommand.NextScreenName];
            if (nextScreen != null && _navigationStack.Count != 0 && _navigationStack.Contains(nextScreen))
            {
                isReturnToPrevious = true;
                while (_navigationStack.Last() != nextScreen && _navigationStack.Count != 0)
                {
                    var peek = _navigationStack.Last();
                    peek.MoveToInitialPosition();
                    _navigationStack.Remove(peek);
                }

                nextScreen.GotFocus();
            }

            return isReturnToPrevious;
        }

        private void Show(ScreenName screenName, object extraData = null, bool withAnim = true, bool delayed = false)
        {
            Debug.Log("Try to show screen " + screenName);

            if (!_availableScreens.ContainsKey(screenName))
            {
                Debug.Log($"Screen {screenName} has not loaded yet. Try to load.");
                GetNewScreen(screenName, () => Show(screenName, extraData, withAnim, delayed));
                return;
            }

            if (_currentShowAnimatedScreenView == _availableScreens[screenName])
            {
                Debug.LogError("You are trying to show the same screen again. " + screenName);
                return;
            }

            if (_currentShowAnimatedScreenView != null && withAnim)
            {
                Debug.Log("animation of another screen is still running (WAIT)");
                if (_waitingScreenInfos.Any(info => info.name == screenName)) return;
                _waitingScreenInfos.Enqueue(new NextScreenInfo
                {
                    name = screenName,
                    extraData = extraData,
                    withAnim = withAnim
                });
                return;
            }

            var screenView = _availableScreens[screenName];
            CheckRootScreenShown(screenView);
            AddScreenToStack(screenView);

            if (screenView.OnShowTransitionAnimation != null && withAnim)
            {
                _currentShowAnimatedScreenView = screenView;
                screenView.InvokeShowWith(extraData);
                screenView.PerformShowAnimationWhenReady(
                    delegate
                    {
                        _currentShowAnimatedScreenView = null;
                        if (_nextScreenInfos.Count <= 0) return;
                        var nextScreenInfo = _nextScreenInfos.Dequeue();
                        nextScreenInfo.IsDelayed = false;
                        ExecuteNavigationCommand(nextScreenInfo);
                    });
            }
            else
            {
                screenView.ShowOnPosition(extraData);
            }
        }

        private void CheckRootScreenShown(ScreenView screenView)
        {
            if (!screenView.IsRootScreen) return;
            Debug.Log("Root screen shown. Close all screens");

            for (int i = _navigationStack.Count - 1; i >= 0; i--)
            {
                if (screenView != _navigationStack[i])
                {
                    Close(_navigationStack[i]);
                }
            }
        }

        private void LogStack()
        {
            /*if (_navigationStack.Count == 0)
            {
                Debug.Log("-----Stack is empty!-----");
            }
            else
            {
                Debug.Log("-----Now in stack:-----");
                foreach (var screen in _navigationStack)
                {
                    Debug.Log(screen.name);
                }
            }*/
        }

        private void AddScreenToStack(ScreenView screenView)
        {
            if (_navigationStack.Count != 0 && _navigationStack.Contains(screenView))
            {
                Debug.LogWarning("Can not add screen " + screenView.name + " to stack because they already in");
                LogStack();
                return;
            }

            if (screenView.ShouldPutInNavStack)
            {
                _navigationStack.Add(screenView);
                Debug.Log("Screen ADDED to stack " + screenView.name);
            }

            LogStack();
        }

        private void RemoveScreenFromStack(ScreenView screenView, bool nextScreenInQueue = false)
        {
            if (_navigationStack.Count == 0 || !screenView.ShouldPutInNavStack) return;

            if (_navigationStack.Contains(screenView))
            {
                _navigationStack.Remove(screenView);
                screenView.LostFocus();
                screenView.InactivateScreenWithDelay(1f);
                if (_waitingScreenInfos.Count > 0)
                {
                    var waitingScreenInfo = _waitingScreenInfos.Dequeue();
                    Show(waitingScreenInfo.name, waitingScreenInfo.extraData);
                }
                else if (_navigationStack.Count > 0 && !nextScreenInQueue)
                {
                    //move focus to previous screen
                    var activeScreen = _navigationStack.Last();
                    activeScreen.GotFocus();
                }

                Debug.Log("Screen REMOVED from stack " + screenView.name);
                if (_navigationStack.Count > 0)
                {
                    //check previous screen for panels
                    var activeScreen = _navigationStack.Last();
                    Debug.Log("After close now screen name: " + activeScreen.gameObject.name);
                }
            }

            LogStack();
        }

        private void Close(ScreenView screenView, bool nextScreenInQueue = false, bool withAnim = true)
        {
            if (!_navigationStack.Contains(screenView)) return;
            if (screenView.ShouldPutInNavStack) RemoveScreenFromStack(screenView, nextScreenInQueue);

            if (screenView.OnHideTransitionAnimation == null || !withAnim)
            {
                screenView.MoveToInitialPosition();
            }
            else
            {
                screenView.PerformHideAnimation(delegate { screenView.MoveToInitialPosition(); });
            }
        }

        private void ForceCloseCurrentScreen()
        {
            if (_navigationStack.Count == 0) return;
            var peek = _navigationStack.Last();
            peek.OnHideTransitionAnimation?.KillAnim();
            peek.OnShowTransitionAnimation?.KillAnim();
            peek.MoveToInitialPosition();
            RemoveScreenFromStack(peek);
        }

        private void ForceCloseAllScreens()
        {
            Debug.Log("Force close all screens");
            int stackCount = _navigationStack.Count;
            while (stackCount-- != 0)
            {
                ForceCloseCurrentScreen();
            }
        }

        public void HideScreenInformation(IScreenView screenView)
        {
            List<CanvasGroup> infoBlocks = screenView.InfoBlocks;
            if (infoBlocks.Count == 0) return;
            MainThreadDispatcher.StartCoroutine(_showInformationProcess);
            foreach (var infoBlock in infoBlocks)
            {
                infoBlock.alpha = 0;
                infoBlock.blocksRaycasts = false;
            }
        }

        public void ShowScreenInformation(IScreenView screenView)
        {
            List<CanvasGroup> infoBlocks = screenView.InfoBlocks;
            if (infoBlocks.Count == 0 || infoBlocks[0].alpha == 1f) return;
            _showInformationProcess = ShowInformationProcess(infoBlocks);
            MainThreadDispatcher.StartCoroutine(_showInformationProcess);
        }

        private IEnumerator ShowInformationProcess(List<CanvasGroup> infoBlocks)
        {
            while (infoBlocks[0].alpha < 1f)
            {
                yield return new WaitForFixedUpdate();
                foreach (var infoBlock in infoBlocks)
                {
                    infoBlock.alpha = Mathf.Clamp01(infoBlock.alpha + 0.1f);
                }
            }

            foreach (var infoBlock in infoBlocks)
            {
                infoBlock.blocksRaycasts = true;
            }
        }

        private class NextScreenInfo
        {
            public ScreenName name;
            public object extraData;
            public bool withAnim;
        }

        /**
         * 1 - cuz MainScreen is always visible and can't be closed, i guess
         */
        public bool IsAnyScreensOpened() => _navigationStack.Count > 1;
    }
}