using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Root_Consume.Services
{
    /// <summary>
    /// 从broker消费消息
    /// 测试Direct消息模型，一个消费成功，一个消费失败最后消失状态，该消费服务模拟消费过程中出现异常
    /// </summary>
    public class MsgFailSuccessTestServerB : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;
        public MsgFailSuccessTestServerB(ILogger<MsgFailSuccessTestServerB> logger, IConnection rabbitConnection)
        {
            _logger = logger;
            _rabbitConnection = rabbitConnection;
            _channel = _rabbitConnection.CreateModel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //设置预取值为1，1比较稳定
            _channel.BasicQos(0, 1, false);
            _channel.ExchangeDeclare("MsgFailSuccessTestServerExchange", "direct");
            var queueName = _channel.QueueDeclare(durable: true, exclusive: false, autoDelete: false).QueueName;
            _channel.QueueBind(queueName, "MsgFailSuccessTestServerExchange", "MsgFailSuccessTestServerRoute");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    //停留10秒钟，首先暂停的原因是为了让消费A先执行，让他执行完之后看一下队列以及消息的状态
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                    _logger.LogInformation($"{DateTime.Now.ToString()},开始接受消息，消息内容为:{Encoding.UTF8.GetString(ea.Body.ToArray())}，路由名：{ea.RoutingKey}，交换机名：{ea.Exchange}，消息唯一标识：{ea.ConsumerTag},消息是否有被消费过:{ea.Redelivered}，队列名：{queueName}");
                    throw new Exception("消费时出现异常了");
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    //否定消息，消息失败后会重新返回到队列中，queue参数如果设置了true而且预取值为1的话，那么此条失败的消息，就会一直回到消息第一个消费，会到之后队列阻塞
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    _logger.LogCritical(ex, $"{DateTime.Now.ToString()}消费出现异常");
                }
            };

            _channel.BasicConsume(queueName, false, consumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}==========over==========");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _rabbitConnection?.Dispose();
        }
    }
}
