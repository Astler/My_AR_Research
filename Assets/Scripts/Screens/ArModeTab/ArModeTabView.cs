using System;
using DG.Tweening;
using JetBrains.Annotations;
using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.ArModeTab
{
    public interface IArModeTabView : IScreenView
    {
        event Action<Vector2> EmptyScreenClicked;
        event Action CollectButtonClicked;
        RectTransform EventsParent { get; }

        void SetDropZoneName([CanBeNull] string zoneName);
        void SetCollectButtonIsActive(bool isActive);
        void SetActivePlayersInZone(int playersCount);
        void SetTimeToNextDrop(string time);
        void SetAvailableRewards(int rewards);
    }

    public class ArModeTabView : ScreenView, IArModeTabView
    {
        [SerializeField] private RectTransform eventsParent;
        [SerializeField] private InfoTextView dropZoneInfo;
        [SerializeField] private Button collectButton;
        [SerializeField] private TextMeshProUGUI playersText, timerText, rewardsText;
        
        private RectTransform _collectButtonRect;

        public event Action<Vector2> EmptyScreenClicked;
        public event Action CollectButtonClicked;

        public RectTransform EventsParent => eventsParent;

        public void SetDropZoneName(string zoneName)
        {
            bool hasZone = zoneName != null;
            dropZoneInfo.SetText(new InfoTextViewInfo
            {
                Text = hasZone ? zoneName : "^go_to_the_event_area".GetTranslation(),
                TextType = hasZone ? InfoTextType.Title : InfoTextType.Hint
            });
        }

        public void SetCollectButtonIsActive(bool isActive)
        {
            collectButton.DOKill();

            if (isActive)
            {
                ShowCollectButton();
            }
            else
            {
                HideCollectButton();
            }
        }

        public void SetActivePlayersInZone(int playersCount)
        {
            playersText.text = "^players".GetTranslation(playersCount);
        }

        public void SetTimeToNextDrop(string time)
        {
            timerText.text = "^next_drop".GetTranslation(time);
        }

        public void SetAvailableRewards(int rewards)
        {
            rewardsText.text = "^drops".GetTranslation(rewards);
        }

        private void HideCollectButton()
        {
            _collectButtonRect.DOAnchorPosY(-_collectButtonRect.sizeDelta.y, 0.5f).OnComplete(() =>
            {
                _collectButtonRect.gameObject.SetActive(false);
            });
        }

        private void ShowCollectButton()
        {
            _collectButtonRect.gameObject.SetActive(true);
            _collectButtonRect.DOAnchorPosY(0f, 0.5f);
        }
        
        private void Awake()
        {
            collectButton.ActionWithThrottle(() => CollectButtonClicked?.Invoke());
            _collectButtonRect = (RectTransform)collectButton.transform;
        }

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