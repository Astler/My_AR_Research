using System;
using System.Collections.Generic;
using Screens;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Map
{
    public class ClickableBehaviour : MonoBehaviour
    {
        private const float ClickDuration = 0.15f;
        private float _mouseDownTime;
        private ScreenNavigationSystem _screenNavigationSystem;
        private bool _interactable = true;
        public event Action Clicked;

        [Inject]
        public void Construct(ScreenNavigationSystem screenNavigationSystem)
        {
            _screenNavigationSystem = screenNavigationSystem;
        }

        public void SetInteractable(bool interactable)
        {
            _interactable = interactable;
        }

        private void OnMouseDown()
        {
            _mouseDownTime = Time.time;
        }

        private void OnMouseUp()
        {
            PointerEventData pointerData = new(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0) return;

            float duration = Time.time - _mouseDownTime;

            if (duration <= ClickDuration && _interactable)
            {
                Clicked?.Invoke();
            }
        }
    }
}