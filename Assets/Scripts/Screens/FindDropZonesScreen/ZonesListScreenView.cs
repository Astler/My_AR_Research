using System;
using Data.Objects;
using DG.Tweening;
using JetBrains.Annotations;
using Screens.Views;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.FindDropZonesScreen
{
    public interface IZonesListScreenView : IScreenView
    {
        event Action MapToPlayerPositionClicked;
        event Action LaunchArClicked;
        event Action SelectedZoneAboutClicked;

        IFindTabBar TabBar { get; }
        RectTransform CardsParent { get; }

        void ShowContentByTab(FindTabType selectedTab);
        void SetDropZoneName([CanBeNull] string zoneName);
        void ShowSelectedZoneInfo([CanBeNull] DropZoneViewInfo viewInfo);
    }

    public class ZonesListScreenView : ScreenView, IZonesListScreenView
    {
        [SerializeField] private FindTabBar findTabBar;
        [SerializeField] private RectTransform listContainer;
        [SerializeField] private Button aboutZoneButton;
        [SerializeField] private Button toMeButton;
        [SerializeField] private Button launchArButton;
        [SerializeField] private DropZoneCardView selectedZoneInfo;
        [SerializeField] private InfoTextView dropZoneInfo;
        [Space] [SerializeField] private GameObject background;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject mapContent;

        private RectTransform _launchArButtonRect;
        private RectTransform _selectedZoneInfoRect;

        public event Action SelectedZoneAboutClicked;
        public event Action MapToPlayerPositionClicked;
        public event Action LaunchArClicked;

        public IFindTabBar TabBar => findTabBar;
        public RectTransform CardsParent => listContainer;

        public void ShowSelectedZoneInfo(DropZoneViewInfo viewInfo)
        {
            if (viewInfo == null)
            {
                HideSelectedZoneInfo();
                return;
            }

            selectedZoneInfo.OnSpawned(viewInfo, null);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_selectedZoneInfoRect);
            ShowSelectedZoneInfo();
        }

        public void SetDropZoneName(string zoneName)
        {
            bool hasZone = zoneName != null;

            launchArButton.DOKill();

            if (hasZone)
            {
                ShowLaunchArButton();
            }
            else
            {
                HideLaunchArButton();
            }

            dropZoneInfo.SetText(new InfoTextViewInfo
            {
                Text = hasZone
                    ? "<sprite=\"DropSprites\" index=0> " + zoneName
                    : "^go_to_the_event_area".GetTranslation(),
                TextType = hasZone ? InfoTextType.Title : InfoTextType.Hint
            });
        }

        private void HideLaunchArButton()
        {
            _launchArButtonRect.DOAnchorPosY(-_launchArButtonRect.sizeDelta.y, 0.5f).OnComplete(() =>
            {
                _launchArButtonRect.gameObject.SetActive(false);
            });
        }

        private void ShowLaunchArButton()
        {
            _launchArButtonRect.gameObject.SetActive(true);
            _launchArButtonRect.DOAnchorPosY(0f, 0.5f);
        }

        private void HideSelectedZoneInfo()
        {
            _selectedZoneInfoRect.DOAnchorPosY(0f, 0.5f);
        }

        private void ShowSelectedZoneInfo()
        {
            _selectedZoneInfoRect.DOAnchorPosY(_selectedZoneInfoRect.sizeDelta.y + 30, 0.5f);
        }

        public void ShowContentByTab(FindTabType selectedTab)
        {
            bool isMapMode = selectedTab == FindTabType.Map;

            background.SetActive(!isMapMode);
            scrollView.gameObject.SetActive(!isMapMode);
            mapContent.SetActive(isMapMode);
        }

        private void Awake()
        {
            toMeButton.ActionWithThrottle(() => MapToPlayerPositionClicked?.Invoke());
            launchArButton.ActionWithThrottle(() => LaunchArClicked?.Invoke());
            aboutZoneButton.ActionWithThrottle(() => SelectedZoneAboutClicked?.Invoke());

            _launchArButtonRect = (RectTransform)launchArButton.transform;
            _selectedZoneInfoRect = (RectTransform)selectedZoneInfo.transform;

            _selectedZoneInfoRect.anchoredPosition = new Vector2(0, 0);
        }
    }
}