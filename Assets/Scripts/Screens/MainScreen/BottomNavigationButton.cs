using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MainScreen
{
    [RequireComponent(typeof(Button))]
    public class BottomNavigationButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [Space, SerializeField] private BottomBarButtonType buttonType;
        
        private Button _button;

        public event Action<BottomBarButtonType> Clicked;

        public BottomBarButtonType Type => buttonType;

        public void SetColor(Color color)
        {
            icon.color = color;
            title.color = color;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.ActionWithThrottle(() => Clicked?.Invoke(buttonType));
        }
    }
}