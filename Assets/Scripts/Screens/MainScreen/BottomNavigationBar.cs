using System;
using UnityEngine;

namespace Screens.MainScreen
{
    public interface IBottomNavigationBar
    {
        event Action<BottomBarButtonType> ClickedNavigationBarButton;

        void SetSelectedButton(BottomBarButtonType type);
    }

    public class BottomNavigationBar : MonoBehaviour, IBottomNavigationBar
    {
        [SerializeField] private BottomNavigationButton[] buttons;
        [Space] [SerializeField] private Color selectedButtonColor, unselectedButtonColor;

        public event Action<BottomBarButtonType> ClickedNavigationBarButton;

        public void SetSelectedButton(BottomBarButtonType type)
        {
            foreach (BottomNavigationButton bottomNavigationButton in buttons)
            {
                bottomNavigationButton.SetColor(type == bottomNavigationButton.Type
                    ? selectedButtonColor
                    : unselectedButtonColor);
            }
        }

        private void Awake()
        {
            foreach (BottomNavigationButton bottomNavigationButton in buttons)
            {
                bottomNavigationButton.Clicked += type => ClickedNavigationBarButton?.Invoke(type);
            }
        }
    }
}