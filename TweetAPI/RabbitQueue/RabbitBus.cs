using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TweetAPI.RabbitQueue
{
    public class RabbitBus:IBus
    {
        private readonly IModel _channel;

        internal RabbitBus(IModel channel)
        {
            _channel = channel;
        }

        public void Send<T>(string queue, T message)
        {
                _channel.QueueDeclare(queue, true, false, false);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = false;

                var output = JsonConvert.SerializeObject(message);
                _channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(output));
        }

        public void  Receive<T>(string queue, Action<T> onMessage)
        {
            _channel.QueueDeclare(queue, true, false, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (s,e) =>
            {
                string jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);
                T item = JsonConvert.DeserializeObject<T>(jsonSpecified);
                onMessage(item);
            };
            _channel.BasicConsume(queue, true, consumer);
        }
    }
}
