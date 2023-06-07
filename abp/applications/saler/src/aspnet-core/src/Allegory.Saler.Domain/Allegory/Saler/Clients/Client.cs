using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.Clients;

public class Client : AuditedAggregateRoot<int>, ICode
{
    public ClientType Type { get; protected set; }
    public string Code { get; protected set; }
    public string Title { get; protected set; }
    public string IdentityNumber { get; protected set; } //As like TaxOrTc field but doesn't have unique constraint
    public string Name { get; protected set; }
    public string Surname { get; protected set; }
    public string TaxOffice { get; protected set; }
    public string Phone1 { get; protected set; }
    public string Phone2 { get; protected set; }
    public string Phone3 { get; protected set; }
    public string EMail { get; protected set; }
    public string KepAddress { get; protected set; }

    protected Client() { }

    internal Client(
        string code,
        string title = default,
        string identityNumber = default,
        ClientType type = default,
        string name = default,
        string surname = default,
        string phone1 = default,
        string phone2 = default,
        string phone3 = default,
        string email = default,
        string kepAddress = default)
    {
        SetCode(code);
        SetTitle(title);
        SetIdentityNumber(identityNumber);
        SetName(name);
        SetSurname(surname);
        SetType(type);
        SetPhone1(phone1);
        SetPhone2(phone2);
        SetPhone3(phone3);
        SetEMail(email);
        SetKepAddress(kepAddress);
    }

    public void SetType(ClientType type)
    {
        switch (type)
        {
            case ClientType.Individual:
                if (Name.IsNullOrWhiteSpace() || Surname.IsNullOrWhiteSpace())
                    throw new BusinessException(SalerDomainErrorCodes.ClientNameAndSurnameMustSet);
                break;
        }

        Type = type;
    }

    internal Client ChangeCode(string code)
    {
        SetCode(code);
        return this;
    }

    private void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), ClientConsts.MaxCodeLength);
        Code = code;
    }

    public void SetTitle(string title)
    {
        Check.Length(title, nameof(Title), ClientConsts.MaxTitleLength);
        Title = title;
    }

    public void SetIdentityNumber(string identityNumber)
    {
        Check.Length(identityNumber, nameof(IdentityNumber), ClientConsts.MaxIdentityNumberLength);
        IdentityNumber = identityNumber;
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(Name), ClientConsts.MaxNameLength);
        Name = name;
    }

    public void SetSurname(string surname)
    {
        Check.Length(surname, nameof(Surname), ClientConsts.MaxSurnameLength);
        Surname = surname;
    }

    public void SetTaxOffice(string taxOffice)
    {
        Check.Length(taxOffice, nameof(TaxOffice), ClientConsts.MaxTaxOfficeLength);
        TaxOffice = taxOffice;
    }

    public void SetPhone1(string phone1)
    {
        Check.Length(phone1, nameof(Phone1), ClientConsts.MaxPhoneLength);
        Phone1 = phone1;
    }

    public void SetPhone2(string phone2)
    {
        Check.Length(phone2, nameof(Phone2), ClientConsts.MaxPhoneLength);
        Phone2 = phone2;
    }

    public void SetPhone3(string phone3)
    {
        Check.Length(phone3, nameof(Phone3), ClientConsts.MaxPhoneLength);
        Phone3 = phone3;
    }

    public void SetEMail(string email)
    {
        Check.Length(email, nameof(EMail), ClientConsts.MaxMailLength);
        EMail = email;
    }

    public void SetKepAddress(string kepAddress)
    {
        Check.Length(kepAddress, nameof(EMail), ClientConsts.MaxMailLength);
        KepAddress = kepAddress;
    }
}
