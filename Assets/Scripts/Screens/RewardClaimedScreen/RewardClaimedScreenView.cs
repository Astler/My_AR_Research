using ExternalTools.ImagesLoader.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardClaimedScreen
{
    public interface IRewardClaimedScreenView : IScreenView
    {
        void ShowReward(RewardScreenViewInfo rewardScreenViewInfo);
    }

    public class RewardClaimedScreenView : ScreenView, IRewardClaimedScreenView
    {
        [SerializeField] private WebImage icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button okButton;
        [SerializeField] private Sprite defaultIcon;
        [SerializeField] private ParticleSystem confettiParticle;

        public void ShowReward(RewardScreenViewInfo rewardScreenViewInfo)
        {
            nameText.text = rewardScreenViewInfo.ItemName;

            icon.sprite = defaultIcon;
            if (rewardScreenViewInfo.ImageUrl != null)
            {
                icon.SetImageUrl(rewardScreenViewInfo.ImageUrl);
            }

            confettiParticle.Stop();
            confettiParticle.Clear();
            confettiParticle.Play();
        }

        private void Awake()
        {
            okButton.onClick.AddListener(CloseScreen);
        }
    }
}