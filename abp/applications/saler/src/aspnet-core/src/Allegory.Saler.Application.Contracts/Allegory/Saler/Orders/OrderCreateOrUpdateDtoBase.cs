using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Orders;

public abstract class OrderCreateOrUpdateDtoBase : ExtensibleEntityDto
{
    [Required]
    [DynamicStringLength(typeof(OrderConsts), nameof(OrderConsts.MaxNumberLength))]
    public string Number { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatu))]
    public OrderStatu Statu { get; set; }

    public string ClientCode { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
