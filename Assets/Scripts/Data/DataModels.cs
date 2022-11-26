using System;

namespace Data
{
    #region base responses

    [Serializable]
    public class ResponseStatus
    {
        [Serializable]
        public class ResponseError
        {
            public string code;
            public string message;
            public string name;

            public override string ToString() => "Code: " + code + "; Name: " + name + "; Message: " + message;
        }

        public bool success;
        public int errorCode;
        public string errorMessage = "";
        public ResponseError error;
        public long timeStamp;

        public override string ToString() => "Response Status\nSuccess is " + success + "; Error Code: " + errorCode +
                                             "\nMessage: " + errorMessage + "Full Error\n" + error;
    }

    [Serializable]
    public class Response
    {
        public ResponseStatus responseStatus;
    }

    #endregion
    
    [Serializable]
    public class SignInResponse
    {
        public string access_token;
        public string token_type;
        public int app_version;
        public string update_url;
        public long current_server_time;
        public UserData user;
    }
    
    [Serializable]
    public class UserData
    {
        public int id;
        public string username;
        public string device_uid;
        public string country;
        public string platform;
    }
    
    [Serializable]
    public class EventData
    {
        public int id;
        public string title;
        public double longitude;
        public double latitude;
        public long start_time;
        public long finish_time;
        public float radius;
        public PrizeData[] prizes;
    }
    
    [Serializable]
    public class EventsData
    {
        public EventData[] events;
    }
    
    [Serializable]
    public class PrizeData
    {
        public int id;
        public int prize_type;
        public int amount;
        public string image;
        public string name;
        public bool is_claimed;
    }
}