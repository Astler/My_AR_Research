using UnityEngine;

namespace Screens.RewardsListScreen
{
    public interface IRewardsListScreenView
    {
        RewardCardView GetRewardsPrefab();
        RectTransform GetListContainer();
    }
    
    public class RewardsListView: MonoBehaviour, IRewardsListScreenView
    {
        [SerializeField] private RewardCardView rewardCardPrefab;
        [SerializeField] private RectTransform listContainer;

        public RewardCardView GetRewardsPrefab() => rewardCardPrefab;

        public RectTransform GetListContainer() => listContainer;

        public void SetActive(bool active) => gameObject.SetActive(active);
    }
}