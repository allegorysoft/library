using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allegory.Saler.Calculations.Product;

[Authorize]
public class ProductCalculatorAppService : SalerAppService, IProductCalculatorAppService
{
    protected IProductCalculator<Discount, CalculableProduct<Discount>, Discount> ProductCalculator { get; }
    public ProductCalculatorAppService(IProductCalculator<Discount, CalculableProduct<Discount>, Discount> productCalculator)
    {
        ProductCalculator = productCalculator;
    }

    public async Task<CalculableProductAggregateRootDto> CalculateAggregateRootAsync(CalculableProductAggregateRootInputDto input)
    {
        var entity = new CalculableProductsAggregateRoot<Discount, CalculableProduct<Discount>, Discount>();

        await ProductCalculator.SetCurrencyInfoAsync(
            entity,
            input.CurrencyCode,
            input.CurrencyRate);

        if (input.Lines != null)
            foreach (var line in input.Lines)
            {
                var entityLine = await CreateCalculableProductAsync(line);
                entityLine = ProductCalculator.Calculate(entityLine);
                entity.Lines.Add(entityLine);
            }

        if (input.Discounts != null)
            ObjectMapper.Map<
                IList<DiscountDto>,
                IList<Discount>>(input.Discounts)
                .ToList()
                .ForEach(inputDiscount =>
                {
                    entity.Discounts.Add(inputDiscount);
                });

        ProductCalculator.CalculateAggregateRoot(entity);

        var result = ObjectMapper.Map<
            CalculableProductsAggregateRoot<
                Discount,
                CalculableProduct<Discount>,
                Discount>,
            CalculableProductAggregateRootDto>(entity);
        result.CurrencyCode = input.CurrencyCode;

        if (input.Lines != null)
            for (int i = 0; i < input.Lines.Count; i++)
                result.Lines[i].CurrencyCode = input.Lines[i].CurrencyCode;

        return result;
    }

    public async Task<CalculableProductDto> CalculateAsync(CalculableProductInputDto input)
    {
        var calculableProduct = await CreateCalculableProductAsync(input);

        var result = ObjectMapper.Map<
            CalculableProduct<Discount>,
            CalculableProductDto>
            (ProductCalculator.Calculate(calculableProduct));
        result.CurrencyCode = input.CurrencyCode;

        return result;
    }

    protected async Task<CalculableProduct<Discount>> CreateCalculableProductAsync(CalculableProductInputDto input)
    {
        CalculableProduct<Discount> calculableProduct = await ProductCalculator.CreateCalculableProductAsync(
            input.Quantity,
            input.Price,
            input.VatRate,
            input.IsVatIncluded,
            total: input.Total,
            discounts: ObjectMapper.Map<
                IList<DiscountDto>,
                IList<Discount>>(input.Discounts),
            deductionCode: input.DeductionCode,
            deductionPart1: input.DeductionPart1,
            deductionPart2: input.DeductionPart2,
            currencyCode: input.CurrencyCode,
            currencyRate: input.CurrencyRate,
            currencyPrice: input.CurrencyPrice,
            currencyTotal: input.CurrencyTotal);

        return calculableProduct;
    }

    public IList<DeductionDto> GetDeductions()
    {
        var deductions = LazyServiceProvider.LazyGetRequiredService<IList<Deduction>>();

        return ObjectMapper.Map<IList<Deduction>, IList<DeductionDto>>(deductions);
    }
}
