using Application.Abstractions.CasheStorage.Mqtt;
using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage.Mqtt
{
    public class UnitStateStore : IUnitStateStore
    {
        public Task<ControlUnitState?> GetAsync(Guid controlUnitId)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(Guid controlUnitId, ControlUnitState state)
        {
            throw new NotImplementedException();
        }
    }
}
