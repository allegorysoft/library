using System.Threading.Tasks;

namespace Allegory.Saler.Data;

public interface ISalerDbSchemaMigrator
{
    Task MigrateAsync();
}
