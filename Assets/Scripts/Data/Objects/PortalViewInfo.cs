using System;
using UnityEngine;

namespace Data.Objects
{
    public class PortalViewInfo
    {
        public string Name;
        public string Distance;
        public float Radius;
        public Vector2 Coordinates;

        public long StartTime;
        public long FinishTime;

        public bool IsActive()
        {
            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            return StartTime < time && time < FinishTime;
        }
    }
}