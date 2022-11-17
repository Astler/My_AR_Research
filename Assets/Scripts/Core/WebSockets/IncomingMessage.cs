using System;
using UnityEngine;

namespace Core.WebSockets
{
    [Serializable]
    public class IncomingMessage
    {
        public string type;
        public string message;

        public string GetType()
        {
            if (String.IsNullOrEmpty(type))
            {
                var m = JsonUtility.FromJson<IncomingMessageData>(message);
                return m.type;
            }
            else
            {
                return type;
            }
        }
    
        public string GetData()
        {
            if (!String.IsNullOrEmpty(message))
            {
                var m = JsonUtility.FromJson<IncomingMessageData>(message);
                return m.data;
            }
            else
            {
                return String.Empty;
            }
        }
    }
  
    [Serializable]
    public class IncomingMessageData
    {
        public string type;
        public string data;
    }

    public enum IncomingMessageType
    {
        ping,
        changed_info_updated,
        new_game_request,
        opponent_finish_match,
        new_friend_request,
        store_info_updated,
        port_timers_updated,
        new_multiplayer_game_request
    }
}