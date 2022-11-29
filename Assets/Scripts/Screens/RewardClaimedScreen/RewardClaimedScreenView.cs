using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardClaimedScreen
{
    public class RewardClaimedScreenView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button okButton;
        [SerializeField] private Sprite defaultIcon;
        public void ShowReward(Sprite sprite, string itemName)
        {
            gameObject.SetActive(true);
            SetIcon(sprite);
            SetName(itemName);
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
            icon.sprite = sprite ? sprite : defaultIcon;
        }

        private void SetName(string itemName)
        {
            nameText.name = itemName;
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