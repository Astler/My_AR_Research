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
        [SerializeField] private Image notificationIcon;
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

        public void SetIsHasNewDrops(bool has)
        {
            notificationIcon.gameObject.SetActive(has);
        }

        private void Awake()
        {
            notificationIcon.gameObject.SetActive(false);
            _button = GetComponent<Button>();
            _button.ActionWithThrottle(() => Clicked?.Invoke(buttonType));
        }
    }
}