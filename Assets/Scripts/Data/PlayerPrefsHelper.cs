using UnityEngine;

namespace Data
{
    public static class PlayerPrefsHelper
    {
        private const string AccessTokenKey = "access_token";
        
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