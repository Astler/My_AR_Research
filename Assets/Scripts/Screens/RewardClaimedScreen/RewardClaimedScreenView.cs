using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardClaimedScreen
{
    public interface IRewardClaimedScreenView: IScreenView
    {
        void ShowReward(RewardScreenViewInfo rewardScreenViewInfo);
    }
    
    public class RewardClaimedScreenView : ScreenView, IRewardClaimedScreenView
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button okButton;
        [SerializeField] private Sprite defaultIcon;
        
        public void ShowReward(RewardScreenViewInfo rewardScreenViewInfo)
        {
            gameObject.SetActive(true);
            SetIcon(rewardScreenViewInfo.Sprite);
            SetName(rewardScreenViewInfo.ItemName);
        }

        private void Awake()
        {
            okButton.onClick.AddListener(CloseScreen);
        }

        private void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite ? sprite : defaultIcon;
        }

        private void SetName(string itemName)
        {
            nameText.text = itemName;
        }
    }
}