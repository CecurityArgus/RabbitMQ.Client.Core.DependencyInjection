using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Extensions
{
    /// <summary>
    /// Extensions that contain business logic of creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    internal static class RabbitMqFactoryExtensions
    {
        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <param name="options">An instance of options <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        /// <remarks>If options parameter is null the method return null too.</remarks>
        internal static IConnection CreateRabbitMqConnection(RabbitMqClientOptions options)
        {
            if (options is null)
            {
                return null;
            }

            var factory = new ConnectionFactory
            {
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                RequestedHeartbeat = options.RequestedHeartbeat,
                DispatchConsumersAsync = true
            };

            if (options.TcpEndpoints?.Any() == true)
            {
                var clientEndpoints = options.TcpEndpoints.Select(x => new AmqpTcpEndpoint(x.HostName, x.Port)).ToList();
                return factory.CreateConnection(clientEndpoints);
            }

            return string.IsNullOrEmpty(options.ClientProvidedName)
                ? CreateConnection(options, factory)
                : CreateNamedConnection(options, factory);
        }

        static IConnection CreateNamedConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList(), options.ClientProvidedName);
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection(options.ClientProvidedName);
        }

        static IConnection CreateConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList());
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection();
        }
    }
}