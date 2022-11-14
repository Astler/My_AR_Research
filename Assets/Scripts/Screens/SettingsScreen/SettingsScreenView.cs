using Screens.Transitions;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.SettingsScreen
{
    public class SettingsScreenView : ScreenView
    {
        [SerializeField] private Button closeButton;
        private void Awake()
        {
            SetShowTransitionAnimation(new AlphaAndScaleTransition(CanvasGroup, 0f, 1f, 1f, 1f, 0.5f));
            SetHideTransitionAnimation(new AlphaAndScaleTransition(CanvasGroup, 1f, 0f, 1f, 1f, 0.5f));
            
            closeButton.ActionWithThrottle(CloseScreen);
        }
    }
}