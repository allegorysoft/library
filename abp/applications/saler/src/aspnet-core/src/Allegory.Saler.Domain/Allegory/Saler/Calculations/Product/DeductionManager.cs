using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Calculations.Product;

public class DeductionManager : ISingletonDependency
{
    private readonly IList<Deduction> _deductions;

    public DeductionManager(IList<Deduction> deductions)
    {
        _deductions = deductions;
    }

    public void CheckDeduction(
        string code,
        short? part1,
        short? part2)
    {
        CheckDeduction(new Deduction
        {
            DeductionCode = code,
            DeductionPart1 = part1,
            DeductionPart2 = part2,
        });
    }

    public void CheckDeduction(IDeduction deduction)
    {
        if (string.IsNullOrWhiteSpace(deduction.DeductionCode))
        {
            if (deduction.DeductionPart1.HasValue || deduction.DeductionPart2.HasValue)
                throw new BusinessException(SalerDomainErrorCodes.DeductionWrong);
        }
        else
        {
            CheckDeductionExists(deduction.DeductionCode);
            CheckDeductionRate(deduction);
        }
    }

    public void CheckDeductionExists(string code)
    {
        var deduction = _deductions.SingleOrDefault(d => d.DeductionCode == code);

        if (deduction == null)
            throw new CodeNotFoundException(typeof(Deduction), code);
    }

    public void CheckDeductionRate(IDeduction deduction)
    {
        if ((deduction?.DeductionPart1 ?? 0) <= 0 || (deduction?.DeductionPart2 ?? 0 )<= 0)
            throw new BusinessException(SalerDomainErrorCodes.DeductionWrong);

        if (deduction.DeductionPart1 > deduction.DeductionPart2)
            throw new BusinessException(SalerDomainErrorCodes.DeductionRateError);
    }

    public decimal CalculateDeduction(IDeduction deduction, decimal amount)
    {
        if (deduction.DeductionPart1 > 0 && deduction.DeductionPart2 > 0)
        {
            var vatExclude = amount / deduction.DeductionPart2.Value * deduction.DeductionPart1.Value;

            amount -= vatExclude;
        }

        return amount;
    }
}
