namespace Allegory.Saler.Calculations.Product;

public interface IDeductionDto
{
    short? DeductionPart1 { get; set; }
    short? DeductionPart2 { get; set; }
    string DeductionCode { get; set; }
}
