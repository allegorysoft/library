using Allegory.Saler.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Calculations.Product;

internal class ProductCalculator<AD, C, D> : IProductCalculator<AD, C, D>
    where AD : Discount
    where C : CalculableProduct<D>
    where D : Discount
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }
    protected DeductionManager DeductionManager { get; }
    protected ICurrencyRepository CurrencyRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyRepository>();

    public ProductCalculator(DeductionManager deductionManager)
    {
        DeductionManager = deductionManager;
    }

    #region Line calculation
    internal void SetCalculate(CalculableProduct<D> entity)
    {
        CalculateBase(entity);
        CalculateVat(entity);
        RoundResult(entity);
    }
    void CalculateBase(CalculableProduct<D> entity)
    {
        CalculateTotal(entity);//Tutar alanı 0'dan büyükse miktarla birim fiyatı tutara göre ayarla
        CalculateDiscounts(entity);
        entity.SetCalculatedTotal(//Kdv dahil/hariç durumuna göre indirimi çıkartıp hesapla
            (entity.IsVatIncluded
            ? entity.LineVatIncludedTotal
            : entity.LineVatExcludedTotal) - entity.DiscountTotal);
    }
    void CalculateTotal(CalculableProduct<D> entity)
    {
        CheckCurrencyBeforeTotal(entity);
        
        if (entity.Total > 0)
        {
            switch (entity.Quantity)
            {
                case 0:
                    entity.SetQuantity(entity.Total / entity.Price);
                    break;
                default:
                    entity.SetPrice(entity.Total / entity.Quantity);
                    break;
            }
        }
        else
            entity.SetTotal(entity.Quantity * entity.Price);

        CheckCurrencyAfterTotal(entity);
    }
    void CheckCurrencyBeforeTotal(CalculableProduct<D> entity)
    {
        //if currency total/price has a value set local total/price
        if (entity.CurrencyRate.HasValue
            && (entity.CurrencyTotal.HasValue || entity.CurrencyPrice.HasValue))
        {
            if (entity.CurrencyTotal > 0)
                entity.SetTotal(entity.CurrencyTotal.Value * entity.CurrencyRate.Value);
            else
                entity.SetPrice(entity.CurrencyPrice.Value * entity.CurrencyRate.Value);
        }
    }
    void CheckCurrencyAfterTotal(CalculableProduct<D> entity)
    {
        //if currency total/price hasn't a value then set currency total/price
        if (entity.CurrencyRate.HasValue)
        {
            if (!entity.CurrencyTotal.HasValue)
                entity.SetCurrencyTotal(entity.Total / entity.CurrencyRate.Value);

            if (!entity.CurrencyPrice.HasValue)
                entity.SetCurrencyPrice(entity.Price / entity.CurrencyRate.Value);
        }
    }
    void CalculateDiscounts(CalculableProduct<D> entity)
    {
        decimal totalDiscount = 0;
        foreach (var discount in entity.Discounts)
        {
            var totalMinusDiscount = entity.Total - totalDiscount;
            discount.Calculate(totalMinusDiscount);
            totalDiscount += discount.Total;
        }

        entity.SetDiscountTotal(totalDiscount);
    }
    void CalculateVat(CalculableProduct<D> entity)
    {
        if (entity.IsVatIncluded)
        {
            entity.SetVatBase(entity.CalculatedTotal / (1 + (entity.VatRate / 100)));
            entity.SetVatAmount(entity.CalculatedTotal - entity.VatBase);
        }
        else
        {
            entity.SetVatBase(entity.CalculatedTotal);
            entity.SetVatAmount(entity.CalculatedTotal * entity.VatRate / 100);
        }

        entity.SetVatAmount(DeductionManager.CalculateDeduction(entity, entity.VatAmount));
    }
    void RoundResult(CalculableProduct<D> entity)
    {
        entity.SetTotal(Math.Round(entity.Total, 2));
        entity.SetCalculatedTotal(Math.Round(entity.CalculatedTotal, 2));
        entity.SetDiscountTotal(Math.Round(entity.DiscountTotal, 2));
        entity.SetVatBase(Math.Round(entity.VatBase, 2));
        entity.SetVatAmount(Math.Round(entity.VatAmount, 2));
        entity.SetPrice(Math.Round(entity.Price, 5));
        
        if (entity.CurrencyRate.HasValue)
        {
            entity.SetCurrencyTotal(Math.Round(entity.CurrencyTotal.Value, 2));
            //entity.SetCurrencyPrice(Math.Round(entity.CurrencyPrice.Value, 2));
        }
    }
    #endregion

    #region Header calculation
    void SetAggregateRootDiscounts(
        CalculableProductsAggregateRoot<AD, C, D> entity,
        IList<Discount> discounts)
    {
        if (discounts == null)
            entity.Discounts.Clear();
        else
        {
            var idList = discounts
            .Where(d => d.Id != default)
            .Select(d => d.Id)
            .ToList();
            entity.Discounts.RemoveAll(line => !idList.Contains(line.Id));

            //TODO there is still bug for discount index. after insert it first show updated rows then shows inserted rows because of id field auto increment
            for (int i = 0; i < discounts.Count; i++)
            {
                var discount = discounts[i];
                if (discount.Id == default)
                {
                    var productDiscount = (AD)Activator.CreateInstance(typeof(AD), true);
                    productDiscount.Set(discount.Rate, discount.Total);
                    entity.Discounts.Insert(i, productDiscount);
                }
                else
                {
                    var d = entity.Discounts.SingleOrDefault(d => d.Id == discount.Id)
                        ?? throw new BusinessException(SalerDomainErrorCodes.DiscountDoesntBelongParent).WithData("discountId", discount.Id);
                    d.Set(discount.Rate, discount.Total);
                }
            }
        }
    }
    void CalculateAggregateRootDiscounts(CalculableProductsAggregateRoot<AD, C, D> entity)
    {
        if (entity.Discounts != null)
        {
            void SetLineDiscountTotal(C entityLine, decimal lineDiscount)
            {
                if (lineDiscount > entityLine.VatBase)
                    throw new BusinessException(SalerDomainErrorCodes.DiscountMustBeBetweenZeroAndTotal);

                entityLine.SetDiscountTotal(lineDiscount + entityLine.DiscountTotal);
            }
            void CalculateCalculatedTotal(C entityLine, decimal lineDiscount)
            {
                //Eğer satır kdv dahilse genel indirimin satır tutarı kdv hariç üstünden hesaplanıyor bu yüzden genel indirim kısmını matrahtan çıkartıp hesaplatmak gerekiyor
                var vatBase = entityLine.VatBase - lineDiscount;
                if (entityLine.IsVatIncluded)
                    entityLine.SetCalculatedTotal(vatBase * (1 + entityLine.VatRate / 100));
                else
                    entityLine.SetCalculatedTotal(vatBase);
            }

            foreach (var discount in entity.Discounts)
            {
                var totalVatBase = entity.Lines.Sum(line => line.VatBase);
                discount.Calculate(totalVatBase);
                var ratio = discount.Total / totalVatBase;
                var remainDiscount = discount.Total;

                var entityLines = entity.Lines.Where(line => line.VatBase > 0).ToList();
                for (int i = 0; i < entityLines.Count; i++)
                {
                    if (remainDiscount <= 0) break;

                    var entityLine = entityLines[i];
                    decimal lineDiscount = ratio * entityLine.VatBase;

                    if (i == entityLines.Count - 1
                        || (remainDiscount - Math.Round(lineDiscount, 2) < 0))
                        lineDiscount = remainDiscount;

                    SetLineDiscountTotal(entityLine, lineDiscount);
                    CalculateCalculatedTotal(entityLine, lineDiscount);
                    CalculateVat(entityLine);
                    RoundResult(entityLine);

                    remainDiscount -= Math.Round(lineDiscount, 2);
                }
            }
        }
    }
    void CalculateAggregateRootTotals(CalculableProductsAggregateRoot<AD, C, D> entity)
    {
        entity.SetTotalGross(entity.Lines.Sum(x => x.VatBase + x.VatAmount));
        entity.SetTotalVatAmount(entity.Lines.Sum(x => x.VatAmount));
        entity.SetTotalVatBase(entity.Lines.Sum(x => x.VatBase));
        entity.SetTotalDiscount(entity.Lines.Sum(x => x.DiscountTotal));
    }
    void RoundAggregateRootTotals(CalculableProductsAggregateRoot<AD, C, D> entity)
    {
        entity.SetTotalGross(Math.Round(entity.TotalGross, 2));
        entity.SetTotalVatAmount(Math.Round(entity.TotalVatAmount, 2));
        entity.SetTotalVatBase(Math.Round(entity.TotalVatBase, 2));
        entity.SetTotalDiscount(Math.Round(entity.TotalDiscount, 2));
    }
    #endregion

    #region Implementation
    public void CalculateAggregateRoot(CalculableProductsAggregateRoot<AD, C, D> entity)
    {
        CalculateAggregateRootDiscounts(entity);
        CalculateAggregateRootTotals(entity);
        RoundAggregateRootTotals(entity);
    }
    public void CalculateAggregateRoot(
        CalculableProductsAggregateRoot<AD, C, D> entity,
        IList<Discount> discounts)
    {
        //This method for set discount by discounts parameter if discounts parameter is null then clear all entity.Discounts 
        SetAggregateRootDiscounts(entity, discounts);
        CalculateAggregateRoot(entity);
    }

    public CalculableProduct<D> Calculate(CalculableProduct<D> calculableProduct)
    {
        var entity = calculableProduct.Clone();
        SetCalculate(entity);
        return entity;
    }

    public async Task<CalculableProduct<D>> CreateCalculableProductAsync(
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
        decimal? currencyTotal = default)
    {
        var calculableProduct = new CalculableProduct<D>(
            quantity,
            price,
            vatRate,
            isVatIncluded,
            total,
            deductionCode,
            deductionPart1,
            deductionPart2);

        DeductionManager.CheckDeduction(calculableProduct);
        await SetLineCurrencyInfoAsync(
            calculableProduct,
            currencyCode,
            currencyRate,
            currencyPrice,
            currencyTotal);
        SetDiscounts(calculableProduct, discounts);

        return calculableProduct;
    }

    public async Task SetCurrencyInfoAsync(
        CalculableProductsAggregateRoot<AD, C, D> entity,
        string currencyCode,
        decimal? currencyRate)
    {
        if (currencyCode.IsNullOrWhiteSpace())
        {
            if (currencyRate.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.CurrencyInformationIncorrect);

            entity.CurrencyId = null;
            entity.SetCurrencyRate(null);
        }
        else
        {
            if (!currencyRate.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.CurrencyInformationIncorrect);

            var currency = await CurrencyRepository.GetByCodeAsync(
                currencyCode,
                includeDetails: false);

            entity.CurrencyId = currency.Id;
            entity.SetCurrencyRate(currencyRate);
        }
    }
    #endregion

    internal async Task CalculateLineAsync(
        CalculableProduct<D> line,
        decimal quantity,
        decimal price = default,
        decimal vatRate = default,
        bool isVatIncluded = default,
        decimal total = default,
        IList<Discount> discounts = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default,
        string currencyCode = default,
        decimal? currencyRate = default,
        decimal? currencyPrice = default,
        decimal? currencyTotal = default)
    {
        line.SetCalculateInput(
            quantity,
            price: price,
            vatRate: vatRate,
            isVatIncluded: isVatIncluded,
            total: total,
            deductionCode: deductionCode,
            deductionPart1: deductionPart1,
            deductionPart2: deductionPart2);

        DeductionManager.CheckDeduction(line);
        await SetLineCurrencyInfoAsync(
            line,
            currencyCode,
            currencyRate,
            currencyPrice,
            currencyTotal);
        SetDiscounts(line, discounts);
        SetCalculate(line);
    }

    async Task SetLineCurrencyInfoAsync(
        CalculableProduct<D> line,
        string currencyCode = default,
        decimal? currencyRate = default,
        decimal? currencyPrice = default,
        decimal? currencyTotal = default)
    {
        if (currencyCode.IsNullOrWhiteSpace())
        {
            if (currencyRate.HasValue || currencyPrice.HasValue || currencyTotal.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.CurrencyInformationIncorrect);

            line.CurrencyId = null;
            line.SetCurrencyRate(null);
            line.SetCurrencyPrice(null);
            line.SetCurrencyTotal(null);
        }
        else
        {
            if (!currencyRate.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.CurrencyInformationIncorrect);

            var currency = await CurrencyRepository.GetByCodeAsync(
                currencyCode,
                includeDetails: false);

            line.CurrencyId = currency.Id;
            line.SetCurrencyRate(currencyRate);
            line.SetCurrencyPrice(currencyPrice);
            line.SetCurrencyTotal(currencyTotal);
        }
    }

    void SetDiscounts(
        CalculableProduct<D> line,
        IList<Discount> discounts)
    {
        if (discounts == null)
            line.Discounts.Clear();
        else
        {
            var idList = discounts
                .Where(d => d.Id != default)
                .Select(d => d.Id)
                .ToList();

            line.Discounts.RemoveAll(line => !idList.Contains(line.Id));

            //TODO there is still bug for discount index. after insert it first show updated rows then shows inserted rows because of id field auto increment
            for (int i = 0; i < discounts.Count; i++)
            {
                var discount = discounts[i];
                if (discount.Id == default)
                {
                    var productDiscount = (D)Activator.CreateInstance(typeof(D), true);
                    productDiscount.Set(discount.Rate, discount.Total);
                    line.Discounts.Insert(i, productDiscount);
                }
                else
                {
                    var d = line.Discounts.SingleOrDefault(d => d.Id == discount.Id)
                        ?? throw new BusinessException(SalerDomainErrorCodes.DiscountDoesntBelongParent).WithData("discountId", discount.Id);
                    d.Set(discount.Rate, discount.Total);
                }
            }
        }
    }
}
