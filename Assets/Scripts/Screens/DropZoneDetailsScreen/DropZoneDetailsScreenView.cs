using System;
using Data.Objects;
using Screens.FindDropZonesScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.DropZoneDetailsScreen
{
    public interface IDropZoneDetailsScreenView : IScreenView
    {
        event Action ShareClicked;
        event Action OpenNativeMapsClicked;
        event Action OpenArMapsClicked;

        RectTransform RewardsListParent { get; }
        void ConfigureView(DropZoneViewInfo viewInfo);
    }

    public class DropZoneDetailsScreenView : ScreenView, IDropZoneDetailsScreenView
    {
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private DropZoneCardView selectedZoneInfo;
        [SerializeField] private Button backButton;
        [SerializeField] private Button shareButton;
        [SerializeField] private Button openWithNativeMaps;
        [SerializeField] private Button openArMaps;
        [SerializeField] private TextMeshProUGUI startText, endText;

        public event Action ShareClicked;
        public event Action OpenNativeMapsClicked;
        public event Action OpenArMapsClicked;

        public RectTransform RewardsListParent => listContainer;

        public void ConfigureView(DropZoneViewInfo viewInfo)
        {
            selectedZoneInfo.OnSpawned(viewInfo, null);
            
            DateTime startDateTime = DateTimeOffset.FromUnixTimeSeconds(viewInfo.StartTime).DateTime;
            DateTime endDateTime = DateTimeOffset.FromUnixTimeSeconds(viewInfo.FinishTime).DateTime;

            startText.text = "^start_info".GetTranslation(startDateTime.ToString("dd MMMM yyyy, hh:MM"));
            endText.text = "^finish_info".GetTranslation(endDateTime.ToString("dd MMMM yyyy, hh:MM"));
        }

        private void Awake()
        {
            backButton.ActionWithThrottle(CloseScreen);
            shareButton.ActionWithThrottle(() => ShareClicked?.Invoke());
            openWithNativeMaps.ActionWithThrottle(() => OpenNativeMapsClicked?.Invoke());
            openArMaps.ActionWithThrottle(() => OpenArMapsClicked?.Invoke());
        }
    }
}