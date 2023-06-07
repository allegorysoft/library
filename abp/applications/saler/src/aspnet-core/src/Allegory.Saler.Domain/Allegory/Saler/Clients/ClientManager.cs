using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Clients;

public class ClientManager : SalerDomainService
{
    protected IClientRepository ClientRepository { get; }

    public ClientManager(IClientRepository clientRepository)
    {
        ClientRepository = clientRepository;
    }

    public async Task<Client> CreateAsync(
        string code,
        string title = default,
        string identityNumber = default)
    {
        await CheckClientExistsAsync(code);

        var client = new Client(
            code,
            title: title,
            identityNumber: identityNumber);

        return client;
    }

    public async Task CheckClientExistsAsync(
        string code,
        int? clientId = default)
    {
        Expression<Func<Client, bool>> expression = client => client.Code == code;

        if (clientId != default)
            expression = expression.And(client => client.Id != clientId);

        var clientExists = await ClientRepository.AnyAsync(expression);

        if (clientExists)
            throw new CodeAlreadyExistsException(typeof(Client), code);
    }

    public async Task ChangeCodeAsync(
       Client client,
       string newCode)
    {
        await CheckClientExistsAsync(newCode, client.Id);
        client.ChangeCode(newCode);
    }
}
