using UnityEngine;

namespace Prototype.Data
{
    public static class PlayerPrefsHelper
    {
        private const string CoinsKey = "coins";

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