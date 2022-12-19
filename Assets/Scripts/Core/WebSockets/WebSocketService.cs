using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.WebSocket;
using Data;
using UniRx;
using UnityEngine;
using Utils;

namespace Core.WebSockets
{
    public class WebSocketService : IWebSocketService
    {
        private WebSocket _webSocket;
        private readonly BoolReactiveProperty _isConnectedToSocket = new();
        public IReadOnlyReactiveProperty<bool> IsConnectedToSocket => _isConnectedToSocket;
        public event Action<WebSocket, IncomingMessage> OnIncomingMessageReceived;
        private string _userToken;

        private static readonly List<string> IncomingMessagesTypeNames =
            Enum.GetNames(typeof(IncomingMessagesTypes)).ToList();

        public void SubscribeToEventSessionChannel(int eventId)
        {
            MessageData data = new()
            {
                command = "subscribe",
                identifier = "{\"channel\":\"EventSessionChannel\", \"event_id\": \"" + eventId + "\"}"
            };
            _webSocket.Send(JsonUtility.ToJson(data));
        }

        public void Connect(string userToken)
        {
            _userToken = userToken;
            string wsUrl = GetWsEndpoint();
            Debug.Log($"WebSocket try connect to {wsUrl}");
            _webSocket = new WebSocket(new Uri(wsUrl));
            InitializeCallbacks();
            _webSocket.Open();
        }

        public WebSocket CurrentConnection()
        {
            return _webSocket;
        }

        public void SendMessage<T>(WebSocketMessage<T> message)
        {
            Debug.Log($"Send WebSocket message: {message.ToMessage()}");
            _webSocket.Send(message.ToMessage());
        }

        public void Disconnect()
        {
            if (_webSocket != null)
            {
                _webSocket.OnOpen -= OnWebSocketOpen;
                _webSocket.OnMessage -= OnMessageReceived;
                _webSocket.OnClosed -= OnWebSocketClosed;
                _webSocket.OnError -= OnError;
                _webSocket.Close();
            }
        }

        private void SubscribeToGameSessionChannel()
        {
            var data = new MessageData()
            {
                command = "subscribe",
                identifier = "{\"channel\":\"GameSessionChannel\"}"
            };
            _webSocket.Send(JsonUtility.ToJson(data));
        }

        private void SubscribeToGeneralChannel()
        {
            var data = new MessageData()
            {
                command = "subscribe",
                identifier = "{\"channel\":\"GeneralChannel\"}"
            };
            _webSocket.Send(JsonUtility.ToJson(data));
        }

        private void InitializeCallbacks()
        {
            _webSocket.OnOpen += OnWebSocketOpen;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.OnClosed += OnWebSocketClosed;
            _webSocket.OnError += OnError;
        }

        private void OnWebSocketOpen(WebSocket webSocket)
        {
            Debug.Log("WebSocket is now Open!");
            _isConnectedToSocket.Value = true;
            SubscribeToGameSessionChannel();
            SubscribeToGeneralChannel();
            _webSocket.StartPingThread = true;
        }

        private void OnMessageReceived(WebSocket webSocket, string message)
        {
            var im = JsonUtility.FromJson<IncomingMessage>(message);
            if (IncomingMessagesTypeNames.Contains(im.GetType()))
            {
                OnIncomingMessageReceived?.Invoke(webSocket, im);
            }
            else if (im.GetType() != IncomingMessageType.ping.ToString())
            {
                Debug.Log("Text Message received from WebSocket server: " + message);
            }
            else
            {
                Debug.Log($"{im.GetType()} received from WebSocket server: " + message);
            }
        }

        private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
        {
            Debug.Log("WebSocket is now Closed!");
        }

        private void OnError(WebSocket ws, string error)
        {
            Debug.LogError("WebSocket Error: " + error);
            if (!ws.IsOpen)
            {
                _isConnectedToSocket.Value = false;
                TryReconnect();
            }
        }

        private void TryReconnect()
        {
            Disconnect();
            string wsUrl = GetWsEndpoint();
            Debug.Log($"WebSocket try reconnect to {wsUrl}");
            _webSocket = new WebSocket(new Uri(wsUrl));
            InitializeCallbacks();
            _webSocket.Open();
        }

        private static string GetMainEndpointUri()
        {
            return GlobalConstants.EnvironmentType switch
            {
                EnvironmentType.localhost => "http://localhost:3000",
                EnvironmentType.dev => "https://manna-drops.themindstudios.com",
                EnvironmentType.staging => "https://manna-drops.themindstudios.com",
                EnvironmentType.prod => "https://manna-drops.themindstudios.com",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetWsEndpoint()
        {
            return $"{GetMainEndpointUri()}/cable?token={_userToken}";
        }
    }
}