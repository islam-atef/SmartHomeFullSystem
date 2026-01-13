using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.CasheStorage.Mqtt
{
    public interface IUnitStateStore
    {
        Task SaveAsync(Guid controlUnitId, ControlUnitState state);
        Task<ControlUnitState?> GetAsync(Guid controlUnitId);
    }
}
