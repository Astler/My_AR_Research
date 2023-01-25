using System;
using UnityEngine;

namespace Screens.ArModeTab
{
    public interface IArModeTabView : IScreenView
    {
        event Action<Vector2> EmptyScreenClicked;
        RectTransform EventsParent { get; }
    }

    public class ArModeTabView : ScreenView, IArModeTabView
    {
        [SerializeField] private RectTransform eventsParent;

        public event Action<Vector2> EmptyScreenClicked;
        public RectTransform EventsParent => eventsParent;

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !Utils.Utils.IsPointerOverUIObject())
            {
                EmptyScreenClicked?.Invoke(Input.mousePosition);
            }
#else
            if (Input.touches.Length > 0 && !Utils.Utils.IsPointerOverUIObject())
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    EmptyScreenClicked?.Invoke(Input.GetTouch(0).position);
                }
            }
#endif
        }
    }
}