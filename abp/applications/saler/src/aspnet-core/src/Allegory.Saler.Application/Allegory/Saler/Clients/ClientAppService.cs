using Allegory.Saler.Orders;
using Allegory.Saler.Permissions;
using Allegory.Saler.UnitPrices;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectExtending;

namespace Allegory.Saler.Clients;

[Authorize(SalerPermissions.ClientManagement.Client.Default)]
public class ClientAppService : SalerAppService, IClientAppService
{
    protected IClientRepository ClientRepository { get; }
    protected ClientManager ClientManager { get; }
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected IOrderRepository OrderRepository => LazyServiceProvider.LazyGetRequiredService<IOrderRepository>();

    public ClientAppService(
        IClientRepository clientRepository,
        ClientManager clientManager)
    {
        ClientRepository = clientRepository;
        ClientManager = clientManager;
    }

    public virtual async Task<PagedResultDto<ClientDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Client.Id);

        var query = await ClientRepository.GetQueryableAsync();

        return await query.PageResultAsync<Client, ClientDto>(AsyncExecuter, ObjectMapper, input);
    }

    public virtual async Task<ClientWithDetailsDto> GetAsync(int id)
    {
        var client = await ClientRepository.GetAsync(id);
        return ObjectMapper.Map<Client, ClientWithDetailsDto>(client);
    }

    public virtual async Task<ClientWithDetailsDto> GetByCodeAsync(string code)
    {
        var client = await ClientRepository.GetByCodeAsync(code);
        return ObjectMapper.Map<Client, ClientWithDetailsDto>(client);
    }

    [Authorize(SalerPermissions.ClientManagement.Client.Create)]
    public virtual async Task<ClientWithDetailsDto> CreateAsync(ClientCreateDto input)
    {
        Client client = await ClientManager.CreateAsync(input.Code);

        await SetClientBaseAsync(client, input);

        await ClientRepository.InsertAsync(client, autoSave: true);

        return ObjectMapper.Map<Client, ClientWithDetailsDto>(client);
    }

    [Authorize(SalerPermissions.ClientManagement.Client.Edit)]
    public virtual async Task<ClientWithDetailsDto> UpdateAsync(int id, ClientUpdateDto input)
    {
        Client client = await ClientRepository.GetAsync(id);

        if (client.Code != input.Code)
            await ClientManager.ChangeCodeAsync(client, input.Code);

        await SetClientBaseAsync(client, input);

        await ClientRepository.UpdateAsync(client);

        return ObjectMapper.Map<Client, ClientWithDetailsDto>(client);
    }

    protected virtual async Task SetClientBaseAsync(
        Client client,
        ClientCreateOrUpdateDtoBase input)
    {
        input.MapExtraPropertiesTo(client, MappingPropertyDefinitionChecks.None);
        client.SetTitle(input.Title);
        client.SetIdentityNumber(input.IdentityNumber);
        client.SetName(input.Name);
        client.SetSurname(input.Surname);
        client.SetType(input.Type);
        client.SetTaxOffice(input.TaxOffice);
        client.SetPhone1(input.Phone1);
        client.SetPhone2(input.Phone2);
        client.SetPhone3(input.Phone3);
        client.SetEMail(input.EMail);
        client.SetKepAddress(input.KepAddress);

        await Task.CompletedTask;
    }

    [Authorize(SalerPermissions.ClientManagement.Client.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        if (await OrderRepository.AnyAsync(order => order.ClientId == id))
            throw new ThereIsTransactionRecordException(typeof(Client), typeof(Order), isDelete: true);

        await UnitPriceRepository.DeleteAsync(x => x.ClientId == id);

        await ClientRepository.DeleteAsync(id);
    }
}
