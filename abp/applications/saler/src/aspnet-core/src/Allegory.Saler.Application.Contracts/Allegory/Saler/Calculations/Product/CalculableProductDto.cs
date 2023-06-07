using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Allegory.Saler.Calculations.Product;

public class CalculableProductDto : EntityDto, IDeductionDto
{
    public decimal Quantity { get; set; }//Miktar

    public decimal Price { get; set; }//Birim fiyat

    public decimal VatRate { get; set; }//Kdv oranı %8, %18, vb..

    public bool IsVatIncluded { get; set; }//Kdv dahil mi

    public decimal Total { get; set; }//Tutar

    public decimal DiscountTotal { get; set; }//Toplam indirim tutarı

    public decimal CalculatedTotal { get; set; }//Hesaplanmış tutar

    public decimal VatBase { get; set; }//Kdv matrahı(kdv oranına göre hesaplanacak kdv tutarının alacağı temel tutar)

    public decimal VatAmount { get; set; }//Kdv tutarı 1000₺ ürünün %18 kdvsinde 180₺ gibi

    public IList<DiscountDto> Discounts { get; set; }

    public short? DeductionPart1 { get; set; }

    public short? DeductionPart2 { get; set; }

    public string DeductionCode { get; set; }

    public string CurrencyCode { get; set; }

    public decimal? CurrencyRate { get; set; }
    
    public decimal? CurrencyPrice { get; set; }

    public decimal? CurrencyTotal { get; set; }
}
