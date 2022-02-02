using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AASA.NetCore.IDomainService.Panic.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
       where T : class;

        Task PublishAsync<T>(T message, string exchangeName, string exchangeType, string routeKey)
     where T : class;
    }
}
