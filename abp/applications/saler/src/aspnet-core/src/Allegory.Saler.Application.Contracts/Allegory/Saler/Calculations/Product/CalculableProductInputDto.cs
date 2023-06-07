using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProductInputDto : EntityDto, IDeductionDto
{
    public decimal Quantity { get; set; }//Miktar

    public decimal Price { get; set; }//Birim fiyat

    public decimal VatRate { get; set; }//Kdv oranı %8, %18, vb..

    public bool IsVatIncluded { get; set; }//Kdv dahil mi

    public decimal Total { get; set; }//Tutar

    public IList<DiscountDto> Discounts { get; set; }

    public short? DeductionPart1 { get; set; }

    public short? DeductionPart2 { get; set; }

    [DynamicStringLength(typeof(DeductionConsts), nameof(DeductionConsts.MaxDeductionCodeLength))]
    public string DeductionCode { get; set; }

    public string CurrencyCode { get; set; }
   
    public decimal? CurrencyRate { get; set; }

    public decimal? CurrencyPrice { get; set; }
    
    public decimal? CurrencyTotal { get; set; }

}