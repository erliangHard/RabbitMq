using Microsoft.Extensions.Configuration;
using System;

namespace Root_Options
{
    public class RabbitMqOption
    {
        public RabbitMqOption(IConfiguration config)
        {
            var section = config.GetSection("RabbitMq");
            section.Bind(this);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 虚拟Host
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
    }
}
