using System.Collections.Generic;
using Allegory.Standard.Filter.Enums;
using Allegory.Standard.Filter.Properties;

namespace Allegory.Standard.Filter.Concrete;

public static class SortExtension
{
    public static string GetFilterQuery(this Sort sort, OrderDirectionCombine orderDirectionCombine = OrderDirectionCombine.WithOrderBy)
    {
        if (sort == null || string.IsNullOrEmpty(sort.ToString())) return null;

        string filterQuery = sort.ToString();

        switch (orderDirectionCombine)
        {
            case OrderDirectionCombine.WithNone:
                break;
            case OrderDirectionCombine.WithOrderBy:
                filterQuery = filterQuery.Insert(0, " ORDER BY ");
                break;
            case OrderDirectionCombine.WithComma:
                filterQuery = filterQuery.Insert(0, " , ");
                break;
        }

        return filterQuery;
    }

    public static string GetFilterQuery<TEntity>(this Sort sort, OrderDirectionCombine orderDirectionCombine = OrderDirectionCombine.WithOrderBy)
    {
        sort = sort.RemoveSort<TEntity>();

        return sort.GetFilterQuery(orderDirectionCombine);
    }

    public static Sort RemoveSort<TEntity>(this Sort sort)
    {
        if (sort == null) return sort;

        if (sort.IsColumn)
        {
            return typeof(TEntity).GetProperty(sort.Column) == null
                ? throw new FilterException(string.Format(Resource.MemberOfTypeError, sort.Column, typeof(TEntity).FullName))
                : sort;
        }
        else
        {
            List<Sort> sorts = new List<Sort>();
            for (int i = 0; i < sort.Group.Count; i++)
            {
                sort.Group[i] = sort.Group[i].RemoveSort<TEntity>();
                if (sort.Group[i] != null)
                    sorts.Add(sort.Group[i]);
            }

            return sorts.Count > 0 ? new Sort(sorts) : null;
        }
    }
}