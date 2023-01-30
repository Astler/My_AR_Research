using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class GameAssets
    {
        [SerializeField] private Sprite[] userIcons;

        public Sprite GetUserIconById(int responseUserID)
        {
            int totalIcons = userIcons.Length;
            int iconIndex = responseUserID % totalIcons;
            return userIcons[iconIndex];
        }
    }
}