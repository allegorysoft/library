using System;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.ClientUsers;

public class ClientUser : Entity<int>
{
    public int ClientId { get; set; }
    public Guid UserId { get; set; }

    protected ClientUser() { }

    public ClientUser(
        int clientId,
        Guid userId)
    {
        ClientId = clientId;
        UserId = userId;
    }
}
