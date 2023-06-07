namespace Allegory.Saler.Calculations.Product;

public interface IDeduction
{
    string DeductionCode { get; }
    short? DeductionPart1 { get; }
    short? DeductionPart2 { get; }
}
