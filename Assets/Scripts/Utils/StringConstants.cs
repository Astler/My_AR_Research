using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class StringConstants
    {
        private static readonly Dictionary<RuntimePlatform, string> PlatformName = new()
        {
            [RuntimePlatform.Android] = "android",
            [RuntimePlatform.IPhonePlayer] = "ios"
        };
        
        public static string GetPlatformName(RuntimePlatform platform)
        {
            if (PlatformName.ContainsKey(platform)) return PlatformName[platform];
            Debug.LogWarning($"String by type {platform.ToString()} do not found!");
            return PlatformName[RuntimePlatform.Android];
        }
    }
}