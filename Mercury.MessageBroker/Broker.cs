using Mercury.Common.Models;
using Mercury.Common.Services;
using Mercury.MessageBroker.Exceptions;
using Mercury.Plugin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace Mercury.MessageBroker
{
    public class Broker : IMessageBroker, IDisposable
    {
        private readonly ILogger<Broker> _logger;
        IConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly Dictionary<string, string> _consumerTags = new Dictionary<string, string>();
        public Broker(ILogger<Broker> logger, IConnectionFactory mqConnectionFactory)
        {
            _logger = logger;
            _factory = mqConnectionFactory;
            if (_factory is ConnectionFactory)
            {
                ((ConnectionFactory)_factory).DispatchConsumersAsync = true;
            }
            int conn = 0;

            //Retry logic
            while (true)
            {
                try
                {
                    _connection = _factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _logger.LogDebug($"Broker connected to {_connection.Endpoint.HostName} ");
                    break;

                }
                catch (BrokerUnreachableException)
                {
                    conn++;
                    if (conn > 4)
                    {
                        throw;
                    }
                    _logger.LogWarning($"Broker was unreachable. Attempt {conn} ");
                    Thread.Sleep(5000 * conn);
                }
            }


        }

        public void EnqueueServiceRequest(IServiceJobMessage msg)
        {
            publish(msg.ID.ToString(), msg.Service, msg);

        }
        public void EnqueueServiceResponse(IServiceResult msg, string ID)
        {
            publish(ID.ToString(), "correlation", msg);
        }

        public void EnqueueJobCompletion(IJob job)
        {
            publish(job.ID.ToString(), $"completed:{job.ID}", job);
        }


        private void publish<T>(string ID, string queue, T msg)
        {
            _logger.LogDebug($"Enqueuing '{queue}' for {ID}");
            var message = JsonSerializer.Serialize(msg);
            var body = Encoding.UTF8.GetBytes(message);
            _logger.LogDebug($"Message is {message}");
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.CorrelationId = ID;

            _logger.LogDebug($"Enqueuing message {ID}");

            _channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                body: body,
                basicProperties: properties
                );
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        public void RegisterServiceRequestListener(string service, Func<IServiceJobMessage, Task<bool>> receive)
        {
            RegisterListener<ServiceJobMessage>(
                queue: service,
                receive: async (job, ID) => await receive(job),
                exclusive: false,
                durable: true);
        }

        public void RegisterServiceResponseListener(Func<IServiceResult, string, Task<bool>> receive)
        {
            RegisterListener<ServiceResult>(
                queue: "correlation",
                receive: receive,
                exclusive: false,
                durable: true);
        }

        public void RegisterJobCompleteListener(string ID, Func<IJob, Task<bool>> receive)
        {
            RegisterListener<Job>(
                queue: $"completed:{ID}",
                receive: async (job, ID) => await receive(job),
                exclusive: true,
                durable: false
                );
        }
        private void RegisterListener<T>(string queue, Func<T, string, Task<bool>> receive, bool exclusive = false, bool durable = true)
        {
            _logger.LogDebug($"Registering Listener for '{queue}'");
            _channel.QueueDeclare(
                queue: queue,
                durable: durable,
                exclusive: exclusive,
                autoDelete: false);
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                try
                {


                    _logger.LogDebug($"Received message on {queue}. Msg: {args.BasicProperties.MessageId}, CorrID: {args.BasicProperties.CorrelationId}");

                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogDebug($"Body: {message}");

                    var obj = JsonSerializer.Deserialize<T>(message);
                    _logger.LogDebug($"{args.BasicProperties.CorrelationId} corrID was of type {typeof(T).Name}");

                    if (obj == null)
                    {
                        throw new FormatException("Dequeued message could not be deserialized properly. Resulted in null. \r\n" + message);
                    }

                    bool complete = await receive(obj, args.BasicProperties.CorrelationId);
                    _logger.LogDebug($"{args.BasicProperties.MessageId}/{args.BasicProperties.CorrelationId} was processed. result was {complete}");
                    if (complete)
                    {
                        _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving message");
                    throw;
                }

            };

            string tag = _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            _consumerTags.Add(queue, tag);
            _logger.LogDebug($"Registered Listener for '{queue}'. Tag: {tag}");
        }
    }
}