using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Objects
{
    public class ZoneViewInfo
    {
        public string Name;
        public string Distance;
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
            return StartTime < time && time < FinishTime;
        }
    }
}