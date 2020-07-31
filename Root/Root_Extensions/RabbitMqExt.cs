using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Root_Options;
using System;

namespace Root_Extensions
{
    public static class RabbitMqExt
    {
        public static IServiceCollection AddRabbitMqConnection(this IServiceCollection services, IConfiguration config)
        {
            var rabbitMqOption = new RabbitMqOption(config);

            var factory = new ConnectionFactory
            {
                UserName = rabbitMqOption.UserName,
                HostName = rabbitMqOption.HostName,
                Password = rabbitMqOption.Password,
                Port = rabbitMqOption.Port,
                VirtualHost = rabbitMqOption.VirtualHost
            };
            services.AddSingleton(factory);
            services.AddSingleton(factory.CreateConnection());
            return services;
        }
    }
}
