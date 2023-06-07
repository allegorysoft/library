using Allegory.Saler.Clients;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Allegory.Saler.ClientUsers;

[Authorize(IdentityPermissions.Users.Update)]
public class ClientUserAppService : SalerAppService, IClientUserAppService
{
    protected IClientUserRepository ClientUserRepository { get; }

    public ClientUserAppService(IClientUserRepository clientUserRepository)
    {
        ClientUserRepository = clientUserRepository;
    }

    public virtual async Task AddUser(int clientId, Guid userId)
    {
        await LazyServiceProvider
            .LazyGetRequiredService<IClientRepository>()
            .GetAsync(clientId, false);//Checking client exists

        await LazyServiceProvider
            .LazyGetRequiredService<IIdentityUserRepository>()
            .GetAsync(userId, false);//Checking user exists

        var clientUser = await ClientUserRepository.FindAsync(clientUser => clientUser.UserId == userId);

        if (clientUser == null)
        {
            clientUser = new ClientUser(clientId, userId);
            await ClientUserRepository.InsertAsync(clientUser);
        }
        else if (clientUser.ClientId != clientId)
        {
            clientUser.ClientId = clientId;
            await ClientUserRepository.UpdateAsync(clientUser);
        }
    }

    public virtual async Task RemoveUser(Guid userId)
    {
        await ClientUserRepository.DeleteAsync(clientUser => clientUser.UserId == userId);
    }
}
