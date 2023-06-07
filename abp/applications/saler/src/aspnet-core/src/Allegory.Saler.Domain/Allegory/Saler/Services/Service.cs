﻿using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.Services;

public class Service : AuditedAggregateRoot<int>, ICode
{
    public string Code { get; protected set; }
    public string Name { get; protected set; }
    public int UnitGroupId { get; internal set; }
    public string DeductionCode { get; protected set; }
    public short? SalesDeductionPart1 { get; protected set; }
    public short? SalesDeductionPart2 { get; protected set; }
    public short? PurchaseDeductionPart1 { get; protected set; }
    public short? PurchaseDeductionPart2 { get; protected set; }
    public byte SalesVatRate { get; protected set; }
    public byte PurchaseVatRate { get; protected set; }

    protected Service() { }

    internal Service(
        string code,
        int unitGroupId,
        string name = default)
    {
        SetCode(code);
        SetName(name);
        UnitGroupId = unitGroupId;
    }

    internal Service ChangeCode(string code)
    {
        SetCode(code);
        return this;
    }

    private void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), ServiceConsts.MaxCodeLength);
        Code = code;
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(Name), ServiceConsts.MaxNameLength);
        Name = name;
    }

    public void SetSalesVatRate(byte vatRate)
    {
        if (vatRate < 0 || vatRate > 100)
            throw new BusinessException(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);

        SalesVatRate = vatRate;
    }

    public void SetPurchaseVatRate(byte vatRate)
    {
        if (vatRate < 0 || vatRate > 100)
            throw new BusinessException(SalerDomainErrorCodes.VatRateMustBeBetweenZeroAndOneHundred);

        PurchaseVatRate = vatRate;
    }

    internal void SetDeduction(
        string deductionCode,
        short? salesDeductionPart1,
        short? salesDeductionPart2,
        short? purchaseDeductionPart1,
        short? purchaseDeductionPart2)
    {
        DeductionCode = deductionCode;
        SalesDeductionPart1 = salesDeductionPart1;
        SalesDeductionPart2 = salesDeductionPart2;
        PurchaseDeductionPart1 = purchaseDeductionPart1;
        PurchaseDeductionPart2 = purchaseDeductionPart2;
    }
}
