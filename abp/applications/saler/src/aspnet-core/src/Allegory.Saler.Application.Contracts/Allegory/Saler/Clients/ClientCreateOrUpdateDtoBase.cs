using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Allegory.Saler.Clients;

public abstract class ClientCreateOrUpdateDtoBase : ExtensibleEntityDto
{
    [Required]
    [EnumDataType(typeof(ClientType))]
    public ClientType Type { get; set; } = ClientType.Company;

    [Required]
    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxCodeLength))]
    public string Code { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxTitleLength))]
    public string Title { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxIdentityNumberLength))]
    public string IdentityNumber { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxNameLength))]
    public string Name { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxSurnameLength))]
    public string Surname { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxTaxOfficeLength))]
    public string TaxOffice { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxPhoneLength))]
    [Phone]
    public string Phone1 { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxPhoneLength))]
    [Phone]
    public string Phone2 { get; set; }
    
    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxPhoneLength))]
    [Phone]
    public string Phone3 { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxMailLength))]
    [EmailAddress]
    public string EMail { get; set; }

    [DynamicStringLength(typeof(ClientConsts), nameof(ClientConsts.MaxMailLength))]
    [EmailAddress]
    public string KepAddress { get; set; }
}
