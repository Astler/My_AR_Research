using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.RewardsListScreen
{
    public class RewardCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image rewardIcon;

        public void SetupCardData(string name, bool collected)
        {
            title.text = name;
            title.color = collected ? Color.white / 2f : Color.white;
        }

        public void SetRewardIcon(Sprite sprite)
        {
            rewardIcon.sprite = sprite;
        }

        public void DestroyCard() => Destroy(gameObject);
    }
}