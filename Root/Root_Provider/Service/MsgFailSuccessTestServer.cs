using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Root_Provider.Service
{
    /// <summary>
    /// 发布消息给broker
    /// 试Direct消息模型，一个消费成功，一个消费失败最后消失状态
    /// </summary>
    public class MsgFailSuccessTestServer : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _channel;
        public MsgFailSuccessTestServer(ILogger<MsgFailSuccessTestServer> logger, IConnection rabbitConnection)
        {
            _logger = logger;
            _rabbitConnection = rabbitConnection;
            _channel = _rabbitConnection.CreateModel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //未声明队列的需要先去启动消息消费服务
            _channel.ConfirmSelect();
            _channel.ExchangeDeclare("MsgFailSuccessTestServerExchange", "direct");
            var prop = _channel.CreateBasicProperties();
            prop.Persistent = true;
            _channel.BasicPublish("MsgFailSuccessTestServerExchange", "MsgFailSuccessTestServerRoute", null, Encoding.Default.GetBytes("784482163"));
            _channel.BasicAcks += (s, e) =>
            {
                _logger.LogInformation($"{DateTime.Now.ToString()}消息发布成功");
            };

            _channel.BasicNacks += (s, e) =>
            {
                _logger.LogInformation($"{DateTime.Now.ToString()}消息发布失败");
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}==========发布over==========");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _rabbitConnection?.Dispose();
        }
    }
}
