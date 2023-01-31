using System;
using UnityEngine;

namespace Data.Objects
{
    public class RewardViewInfo
    {
        public string Url;
        public int EventId;
        public int Id;
        public int Count;
        public bool IsCollected;
        public string Name;
        public Transform Parent;

        public Action ViewAction;
    }
}