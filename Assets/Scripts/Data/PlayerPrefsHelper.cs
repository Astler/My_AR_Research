using UnityEngine;

namespace Data
{
    public static class PlayerPrefsHelper
    {
        private const string CoinsKey = "coins";
        private const string CustomZonesDataKey = "custom_zones_data";

        public static int Coins
        {
            get => PlayerPrefs.GetInt(CoinsKey, 0);
            set
            {
                PlayerPrefs.SetInt(CoinsKey, value);
                PlayerPrefs.Save();
            }
        }
    }
}