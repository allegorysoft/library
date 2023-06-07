using Allegory.Saler;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories;

public static class RepositoryExtensions
{
    public async static Task<T> FindByCodeAsync<T>(
        this IRepository<T> repository,
        string code,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
        where T : class, ICode, IEntity
    {
        return await repository.FindAsync(entity => entity.Code == code, includeDetails, cancellationToken);
    }

    public async static Task<T> GetByCodeAsync<T>(
        this IRepository<T> repository,
        string code,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
        where T : class, ICode, IEntity
    {
        var entity = await FindByCodeAsync(repository, code, includeDetails, cancellationToken);

        if (entity == null)
            throw new CodeNotFoundException(typeof(T), code);

        return entity;
    }
}
