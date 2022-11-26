using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardsListScreen
{
    public class RewardCardView: MonoBehaviour
    {
        [SerializeField] private Image rewardIcon;
        
        public void SetRewardIcon(Sprite sprite)
        {
            rewardIcon.sprite = sprite;
        }

        public void DestroyCard() => Destroy(gameObject);
    }
}