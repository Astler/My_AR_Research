using UnityEngine;
using UnityEngine.UI;

namespace Screens.HistoryScreen
{
    public interface IHistoryScreenView : IScreenView
    {
        RectTransform GetListContainer();
    }

    public class HistoryScreenView : ScreenView, IHistoryScreenView
    {
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button closeButton;
        public RectTransform GetListContainer() => listContainer;

        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnClose?.Invoke());
        }
    }
}