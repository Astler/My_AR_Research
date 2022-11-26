using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardClaimedScreen
{
    public class RewardClaimedScreenView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button okButton;

        public void ShowReward(Sprite sprite)
        {
            gameObject.SetActive(true);
            SetIcon(sprite);
        }

        private void Awake()
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        private void Start()
        {
            HideScreen();
        }

        private void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        private void OnOkButtonClicked()
        {
            HideScreen();
        }

        private void HideScreen()
        {
            gameObject.SetActive(false);
        }
    }
}