using System;
using UnityEngine;

namespace Screens.MainScreen
{
    public interface IBottomNavigationBar
    {
        event Action<BottomBarButtonType> ClickedNavigationBarButton;

        void SetSelectedButton(BottomBarButtonType type);
        void SetIsTransparentBar(bool isTransparent);
        void SetIsHasNewDrops(bool has);
    }

    public class BottomNavigationBar : MonoBehaviour, IBottomNavigationBar
    {
        [SerializeField] private BottomNavigationButton[] buttons;
        [SerializeField] private CanvasGroup barBackgroundCanvasGroup;
        [Space] [SerializeField] private Color selectedButtonColor, unselectedButtonColor;

        public event Action<BottomBarButtonType> ClickedNavigationBarButton;

        public void SetIsTransparentBar(bool isTransparent)
        {
            barBackgroundCanvasGroup.alpha = isTransparent ? 0.6f : 1f;
        }

        public void SetIsHasNewDrops(bool has)
        {
            foreach (BottomNavigationButton bottomNavigationButton in buttons)
            {
                if (bottomNavigationButton.Type == BottomBarButtonType.MyDrops)
                {
                    bottomNavigationButton.SetIsHasNewDrops(has);
                }
            }
        }

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