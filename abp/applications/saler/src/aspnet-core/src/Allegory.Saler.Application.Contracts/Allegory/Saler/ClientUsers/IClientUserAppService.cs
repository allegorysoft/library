using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Allegory.Saler.ClientUsers;

public interface IClientUserAppService : IApplicationService
{
    Task AddUser(int clientId, Guid userId);
    Task RemoveUser(Guid userId);
}
