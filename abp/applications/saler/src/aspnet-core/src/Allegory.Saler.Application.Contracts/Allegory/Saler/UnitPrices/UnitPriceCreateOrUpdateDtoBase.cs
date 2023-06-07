using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.UnitPrices;

public class UnitPriceCreateOrUpdateDtoBase : ExtensibleEntityDto
{
    [Required]
    [DynamicStringLength(typeof(UnitPriceConsts), nameof(UnitPriceConsts.MaxCodeLength))]
    public string Code { get; set; }

    [Required]
    public string ProductCode { get; set; }

    [Required]
    public string UnitCode { get; set; }

    public string CurrencyCode { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SalesPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PurchasePrice { get; set; }

    [Required]
    public DateTime BeginDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public bool IsVatIncluded { get; set; } = false;

    public string ClientCode { get; set; }
}
