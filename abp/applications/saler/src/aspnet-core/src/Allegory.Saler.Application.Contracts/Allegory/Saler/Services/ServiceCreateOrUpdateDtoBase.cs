using Allegory.Saler.Calculations.Product;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Services;

public abstract class ServiceCreateOrUpdateDtoBase : ExtensibleEntityDto
{
    [Required]
    [DynamicStringLength(typeof(ServiceConsts), nameof(ServiceConsts.MaxCodeLength))]
    public string Code { get; set; }

    [DynamicStringLength(typeof(ServiceConsts), nameof(ServiceConsts.MaxNameLength))]
    public string Name { get; set; }

    [Required]
    public string UnitGroupCode { get; set; }

    [DynamicStringLength(typeof(DeductionConsts), nameof(DeductionConsts.MaxDeductionCodeLength))]
    public string DeductionCode { get; set; }

    public short? SalesDeductionPart1 { get; set; }

    public short? SalesDeductionPart2 { get; set; }

    public short? PurchaseDeductionPart1 { get; set; }

    public short? PurchaseDeductionPart2 { get; set; }

    [Range(0, 100)]
    public byte SalesVatRate { get; set; }

    [Range(0, 100)]
    public byte PurchaseVatRate { get; set; }
}
