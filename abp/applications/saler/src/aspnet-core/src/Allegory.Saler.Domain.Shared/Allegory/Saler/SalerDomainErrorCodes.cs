namespace Allegory.Saler;

public static class SalerDomainErrorCodes
{
    public const string EntityIsUsedCannotDelete = "Saler:00001";
    public const string QuantityCannotZeroOrLess = "Saler:00002";
    public const string PriceCannotLessThanZero = "Saler:00003";
    public const string VatMustBeBetweenZeroAndTotal = "Saler:00004";
    public const string TotalCannotLessThanZero = "Saler:00005";
    public const string DiscountMustBeBetweenZeroAndTotal = "Saler:00006";
    public const string DiscountDoesntBelongParent = "Saler:00007";
    public const string DeductionWrong = "Saler:00008";
    public const string DeductionRateError = "Saler:00009";
    public const string VatRateMustBeBetweenZeroAndOneHundred = "Saler:00010";
    public const string CurrencyRateMustBeGreaterThanZero = "Saler:00011";
    public const string CurrencyInformationIncorrect = "Saler:00012";
    public const string BeginDateCannotBeGreaterThanEndDate = "Saler:00013";
    public const string ReserveWrong = "Saler:00014";
    public const string ReserveQuantityMustGreaterThanZeroAndLessThanQuantity = "Saler:00015";

    public const string UnitDoesnotBelongUnitGroup = "Saler:ProductManagement:00001";
    public const string UnitGroupMustAtLeastOneUnit = "Saler:ProductManagement:00002";
    public const string UnitGroupMustOneMainUnit = "Saler:ProductManagement:00003";
    public const string MainUnitConvFactMustOne = "Saler:ProductManagement:00004";
    public const string UnitConvFactMustSet = "Saler:ProductManagement:00005";
    public const string UnitCannotDivided = "Saler:ProductManagement:00006";

    public const string ClientNameAndSurnameMustSet = "Saler:ClientManagement:00001";
    
}
