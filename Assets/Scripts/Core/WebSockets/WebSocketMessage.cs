using System;
using UnityEngine;

namespace Core.WebSockets
{
    public class WebSocketMessage<T>
    {
        private readonly WebSocketActionTypes _actionType;
        private readonly T _messageData;
        private readonly string _message;

        public WebSocketMessage(WebSocketActionTypes actionType, T messageData)
        {
            _actionType = actionType;
            _messageData = messageData;
            var data = new MessageData()
            {
                command = "message",
                identifier = "{\"channel\":\"GameSessionChannel\"}",
                data = "{\"action\": \"" + actionType + "\", \"message_data\": " + JsonUtility.ToJson(messageData) + "}"
            };
            _message = JsonUtility.ToJson(data);
        }

        public string ToMessage()
        {
            return _message;
        }
    }

    [Serializable]
    public struct MessageData
    {
        public string command;
        public string identifier;
        public string data;
    }

    public enum WebSocketActionTypes
    {
    }
}