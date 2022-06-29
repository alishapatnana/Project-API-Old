using System;

namespace TweetAPI.RabbitQueue
{
    public interface IBus
    {
        void Send<T>(string queue, T message);

        void Receive<T>(string queue, Action<T> onMessage);
    }
}
