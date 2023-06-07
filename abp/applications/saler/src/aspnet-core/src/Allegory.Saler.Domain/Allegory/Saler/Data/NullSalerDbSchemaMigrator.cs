using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Allegory.Saler.Data;

/* This is used if database provider does't define
 * ISalerDbSchemaMigrator implementation.
 */
public class NullSalerDbSchemaMigrator : ISalerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
