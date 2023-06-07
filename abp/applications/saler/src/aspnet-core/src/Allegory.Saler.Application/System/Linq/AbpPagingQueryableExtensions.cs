using Allegory.Standart.Filter.Concrete;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Linq;
using Volo.Abp.ObjectMapping;

namespace System.Linq;

public static class AbpPagingQueryableExtensions
{
    //Order by olmayan ve mapping yapılmayan
    public static async Task<PagedResultDto<T>> PageResultAsync<T>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        IPagedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));

        List<T> items = await asyncExecuter.ToListAsync(query.PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<T>(totalCount, items);
    }

    //Order by olmayan ve mapping yapılan
    public static async Task<PagedResultDto<U>> PageResultAsync<T, U>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        [NotNull] IObjectMapper objectMapper,
        IPagedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));
        Check.NotNull(objectMapper, nameof(objectMapper));

        List<T> items = await asyncExecuter.ToListAsync(query.PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<U>(totalCount, objectMapper.Map<IReadOnlyList<T>, IReadOnlyList<U>>(items));
    }

    //Order by olan ve mapping yapılmayan
    public static async Task<PagedResultDto<T>> PageResultAsync<T>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        IPagedAndSortedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));

        List<T> items = await asyncExecuter.ToListAsync(query.OrderBy(input.Sorting)
                                                             .PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<T>(totalCount, items);
    }

    //Order by olan ve mapping yapılan
    public static async Task<PagedResultDto<U>> PageResultAsync<T, U>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        [NotNull] IObjectMapper objectMapper,
        IPagedAndSortedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));
        Check.NotNull(objectMapper, nameof(objectMapper));

        List<T> items = await asyncExecuter.ToListAsync(query.OrderBy(input.Sorting)
                                                             .PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<U>(totalCount, objectMapper.Map<IReadOnlyList<T>, IReadOnlyList<U>>(items));
    }

    //Order by olan ve mapping yapılmayan
    public static async Task<PagedResultDto<T>> PageResultAsync<T>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        IFilteredPagedAndSortedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));

        var predicate = input.Conditions.GetLambdaExpression<T>();

        query = query.WhereIf(predicate != null, predicate);

        List<T> items = await asyncExecuter.ToListAsync(query.OrderBy(input.Sorting)
                                                             .PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<T>(totalCount, items);
    }

    //Order by olan ve mapping yapılan
    public static async Task<PagedResultDto<U>> PageResultAsync<T, U>(
        [NotNull] this IQueryable<T> query,
        [NotNull] IAsyncQueryableExecuter asyncExecuter,
        [NotNull] IObjectMapper objectMapper,
        IFilteredPagedAndSortedResultRequest input)
    {
        Check.NotNull(query, nameof(query));
        Check.NotNull(asyncExecuter, nameof(asyncExecuter));
        Check.NotNull(objectMapper, nameof(objectMapper));

        var predicate = input.Conditions.GetLambdaExpression<T>();

        query = query.WhereIf(predicate != null, predicate);

        List<T> items = await asyncExecuter.ToListAsync(query.OrderBy(input.Sorting)
                                                             .PageBy(input.SkipCount, input.MaxResultCount));

        var totalCount = await asyncExecuter.LongCountAsync(query);

        return new PagedResultDto<U>(totalCount, objectMapper.Map<IReadOnlyList<T>, IReadOnlyList<U>>(items));
    }
}
