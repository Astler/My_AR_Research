using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.FindDropZonesScreen
{
    [RequireComponent(typeof(Button))]
    public class FindTab : MonoBehaviour
    {
        [SerializeField] private FindTabType findTabType;
        [SerializeField] private Image border, background;

        private Button _button;

        public event Action<FindTabType> Clicked;

        public FindTabType Type => findTabType;

        public void ConfigureView(TabViewInfo tabInfo)
        {
            border.color = tabInfo.Border;
            background.color = tabInfo.Background;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.ActionWithThrottle(() => Clicked?.Invoke(findTabType));
        }
    }
}