using System;
using BestHTTP.WebSocket;

namespace Core.WebSockets
{
    public interface IWebSocketService
    {
        event Action<WebSocket, IncomingMessage> OnIncomingMessageReceived;
        
        void Connect(string userToken);
        void SubscribeToEventSessionChannel(int eventId);
    }
}