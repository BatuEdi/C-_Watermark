using RabbitMQ.Client;

namespace RabbitMq.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ILogger<RabbitMQClientService> _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IModel _channel;
        private IConnection _connection;
        public static string ExchangeName = "ImageDirectExchange";
        public static string QueueName = "queue-watermark-image";
        public static string RoutingWatermark = "watermark-route-image";

        public RabbitMQClientService(ConnectionFactory connectionFactory,ILogger<RabbitMQClientService> logger )
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if(_channel is { IsOpen: true })
            {
                return _channel;
            }
            else
            {
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(ExchangeName, "direct", true, false);
                _channel.QueueDeclare(QueueName,true,false,false,null);
                _channel.QueueBind(queue : QueueName,exchange : ExchangeName, routingKey:RoutingWatermark);
                _logger.LogInformation("RabbitMQ Bağlantısı Kuruldu...");
                return _channel;  
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel?.Close();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("Bağlantı Koptu...");
        }
    }
}
