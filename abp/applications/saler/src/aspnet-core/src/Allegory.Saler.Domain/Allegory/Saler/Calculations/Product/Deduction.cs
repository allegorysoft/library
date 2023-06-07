namespace Allegory.Saler.Calculations.Product;

public class Deduction : IDeduction
{
    public string DeductionCode { get; set; }

    public string DeductionName { get; set; }

    public short? DeductionPart1 { get; set; }

    public short? DeductionPart2 { get; set; }
}
