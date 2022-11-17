using UnityEngine;

namespace Data
{
    public static class PlayerPrefsHelper
    {
        private const string CoinsKey = "coins";
        private const string AccessTokenKey = "access_token";

        public static int Coins
        {
            get => PlayerPrefs.GetInt(CoinsKey, 0);
            set
            {
                PlayerPrefs.SetInt(CoinsKey, value);
                PlayerPrefs.Save();
            }
        }
        
        public static string AccessToken
        {
            get => PlayerPrefs.GetString(AccessTokenKey);
            set
            {
                PlayerPrefs.SetString(AccessTokenKey, value);
                PlayerPrefs.Save();
            }
        }
    }
}