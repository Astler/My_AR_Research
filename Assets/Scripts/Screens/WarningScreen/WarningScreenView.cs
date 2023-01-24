using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Screens.WarningScreen
{
    public interface IWarningScreenView : IScreenView
    {
        event Action OkClicked;
    }

    public class WarningScreenView : ScreenView, IWarningScreenView
    {
        [SerializeField] private Button closeButton;

        public event Action OkClicked;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                OkClicked?.Invoke();
                OnClose?.Invoke();
            });
        }

        private void Update()
        {
            PointerEventData pointerData = new(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);
            
            if (results.Count <= 0) return;
            
            Debug.Log($"ui element = {results.First().gameObject.name}");
        }
    }
}