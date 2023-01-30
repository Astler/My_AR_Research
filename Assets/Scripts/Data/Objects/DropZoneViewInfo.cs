using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Objects
{
    public class DropZoneViewInfo
    {
        public string Name;
        public string ReadableDistance;
        public double OrderDistance;
        public float Radius;
        public Vector2 Coordinates;

        public long StartTime;
        public long FinishTime;
        
        public List<RewardViewInfo> Rewards;
        public int Id;
        public int MinimumDropDistance;
        public int MaximumDropDistance;
        public int InitialBoxes;
        public int MaximumBoxes;

        public bool IsActive()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            return time < FinishTime;
        }
        
        public bool IsOngoing()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            return StartTime < time && time < FinishTime;
        }

        public long GetTimeToStart()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            return StartTime - time;
        }
        
        public IEnumerable<RewardViewInfo> GetGroupedRewards()
        {
            Dictionary<string, RewardViewInfo> groupedRewards = new();

            foreach (RewardViewInfo rewardViewInfo in Rewards)
            {
                if (groupedRewards.ContainsKey(rewardViewInfo.Name))
                {
                    groupedRewards[rewardViewInfo.Name].Count += 1;
                    continue;
                }

                rewardViewInfo.Count = 1;
                groupedRewards[rewardViewInfo.Name] = rewardViewInfo;
            }

            return groupedRewards.Values;
        }
    }
}