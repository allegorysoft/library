using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Allegory.Saler.Calculations.Product;

public class DeductionManager_Tests : SalerDomainTestBase
{
    protected DeductionManager DeductionManager { get; }

    public DeductionManager_Tests()
    {
        DeductionManager = GetRequiredService<DeductionManager>();
    }

    [Fact]
    public void Should_Valid_Deduction()
    {
        var entity = new Deduction()
        {
            DeductionCode = "601",
            DeductionPart1 = 1,
            DeductionPart2 = 2
        };

        DeductionManager.CheckDeduction(entity);
    }

    [Fact]
    public void Should_Calculate_Deduction()
    {
        var entity = new Deduction()
        {
            DeductionPart1 = 1,
            DeductionPart2 = 2
        };

        var result = DeductionManager.CalculateDeduction(entity, 18);

        result.ShouldBe(9);
    }

    [Fact]
    public void Should_Throw_Code_Not_Found_Exception()
    {
        var result = Assert.Throws<CodeNotFoundException>(() =>
        {
            DeductionManager.CheckDeductionExists("xx");
        });

        result.EntityCode.ShouldBe("xx");
    }

    [Fact]
    public void Should_Deduction_Parts_Greater_Than_Zero()
    {

        var result = Assert.Throws<BusinessException>(() =>
        {
            var entity = new Deduction()
            {
                DeductionCode = "601",
                DeductionPart1 = -1,
                DeductionPart2 = 2
            };

            DeductionManager.CheckDeductionRate(entity);
        });

        result.Code.ShouldBe(SalerDomainErrorCodes.DeductionWrong);
    }

    [Fact]
    public void Should_Not_Null_Deduction_Parts_When_Deduction_Code_Not_Null()
    {

        var result = Assert.Throws<BusinessException>(() =>
        {
            var entity = new Deduction()
            {
                DeductionCode = "601"
            };

            DeductionManager.CheckDeductionRate(entity);
        });

        result.Code.ShouldBe(SalerDomainErrorCodes.DeductionWrong);
    }

    [Fact]
    public void Should_Deduction_Part2_Greater_Than_Part1()
    {
        var result = Assert.Throws<BusinessException>(() =>
        {
            var entity = new Deduction()
            {
                DeductionCode = "601",
                DeductionPart1 = 2,
                DeductionPart2 = 1
            };

            DeductionManager.CheckDeductionRate(entity);
        });

        result.Code.ShouldBe(SalerDomainErrorCodes.DeductionRateError);
    }


}
