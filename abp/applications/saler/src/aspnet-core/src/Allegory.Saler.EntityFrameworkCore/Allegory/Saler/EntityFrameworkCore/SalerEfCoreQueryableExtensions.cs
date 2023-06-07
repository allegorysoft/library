using Allegory.Saler.Orders;
using Allegory.Saler.Units;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Allegory.Saler.EntityFrameworkCore;

public static class SalerEfCoreQueryableExtensions
{
    public static IQueryable<UnitGroup> IncludeDetails(
        this IQueryable<UnitGroup> queryable,
        bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Units);
    }

    public static IQueryable<Order> IncludeDetails(
        this IQueryable<Order> queryable,
        bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Lines).ThenInclude(y => y.Discounts)
            .Include(x => x.Discounts);
    }
}
