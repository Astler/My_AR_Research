using System;
using Data;

namespace Core
{
    public interface IApiInterface
    {
        void SignIn(Action<SignInResponse> onSuccess, Action<ResponseStatus> onFailure);
        void GetEventsList(Action<EventsData> onSuccess, Action<ResponseStatus> onFailure);
        void GetAllCollectedRewardsList(Action<CollectedPrizesData> onSuccess, Action<ResponseStatus> onFailure);
        void CollectReward(int zoneId, int rewardId, Action<PrizeCollectResponseData> onSuccess, Action<ResponseStatus> onFailure);
    }
}