namespace Core.WebSockets
{
    public interface IWebSocketService
    {
        void Connect(string userToken);
        void SubscribeToEventSessionChannel(int eventId);
    }
}