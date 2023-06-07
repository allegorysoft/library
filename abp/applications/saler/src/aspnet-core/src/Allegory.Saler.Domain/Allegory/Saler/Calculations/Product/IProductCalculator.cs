using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allegory.Saler.Calculations.Product;

public interface IProductCalculator<AD, C, D>
    where AD : Discount
    where C : CalculableProduct<D>
    where D : Discount
{
    void CalculateAggregateRoot(CalculableProductsAggregateRoot<AD, C, D> entity);

    void CalculateAggregateRoot(
        CalculableProductsAggregateRoot<AD, C, D> aggregateRoot,
        IList<Discount> discounts);

    CalculableProduct<D> Calculate(CalculableProduct<D> calculableProduct);

    Task<CalculableProduct<D>> CreateCalculableProductAsync(
        decimal quantity,
        decimal price,
        decimal vatRate,
        bool isVatIncluded,
        decimal total = default,
        IList<Discount> discounts = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default,
        string currencyCode = default,
        decimal? currencyRate = default,
        decimal? currencyPrice = default,
        decimal? currencyTotal = default);

    Task SetCurrencyInfoAsync(
        CalculableProductsAggregateRoot<AD, C, D> entity,
        string currencyCode,
        decimal? rate);
}
