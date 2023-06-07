using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Allegory.Saler.Units;

public class UnitGroupManager_Tests : SalerDomainTestBase
{
    //AppService testleri buraya taşınacak
    protected UnitGroupManager UnitGroupManager { get; }
    protected IUnitGroupRepository UnitGroupRepository { get; }

    public UnitGroupManager_Tests()
    {
        /*  Exception sırası aşşağıdaki gibi olacak
            Unit oluştururken

	            1-Bölünebilir değilse ve virgüllü değer girdiyse
	            2-Çevrim katsayıları 0 dan küçük mü
	            3-Ana birimse ve çevrim katsayısı 1 e 1 değilse

            UnitGroup oluşturulurken

	            1-UnitGroupCode alanı tekilmi
	            2-UnitCount 0 dan büyük mü
	            3-UnitGroupda 1 tane main unit varmı
        */
        UnitGroupManager = GetRequiredService<UnitGroupManager>();
        UnitGroupRepository = GetRequiredService<IUnitGroupRepository>();
    }

    [Fact]
    public async Task Should_Not_Create_When_Unit_Divisible_False_And_Unit_Conv_Has_Decimal_Places()
    {
        var exception = await Assert.ThrowsAsync<UnitCannotDividedException>(async () =>
        {
            var unit = new Unit(
                "Alt birim",
                1,
                2.1m,
                false);

            await Task.CompletedTask;
        });
    }

    [Fact]
    public async Task Should_Not_Create_When_Unit_Conv_Fact_Equals_Zero()
    {
        var exception = await Assert.ThrowsAsync<UnitConvFactMustSetException>(async () =>
        {
            var unit = new Unit(
                "Alt birim",
                1,
                0,
                false);

            await Task.CompletedTask;
        });
    }

    [Fact]
    public async Task Should_Not_Create_When_Main_Unit_Conv_Fact_Not_Equals_One()
    {
        var exception = await Assert.ThrowsAsync<MainUnitConvFactMustOneException>(
            async () =>
            {
                var unit = new Unit(
                    "Alt birim",
                    1,
                    2,
                    false,
                    mainUnit: true);

                await Task.CompletedTask;
            });
    }

    [Fact]
    public async Task Should_Create_A_Valid_UnitGroup()
    {
        UnitGroup unitGroup = null;
        await WithUnitOfWorkAsync(async () =>
        {
            unitGroup = await UnitGroupManager.CreateAsync(
                "Ana birim-4",
                new List<Unit>()
                {
                    new Unit(
                        "Alt birim-1",
                        1,
                        1,
                        false,
                        mainUnit: true),

                    new Unit(
                        "Alt birim-2",
                        1,
                        1.5m,
                        true)
                });
        });

        unitGroup.Code.ShouldBe("Ana birim-4");
        unitGroup.Units.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Not_Create_Existing_UnitGroup_Code()
    {
        var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var unitGroup = await UnitGroupManager.CreateAsync("Ana birim-3", null);
            });
        });

        exception.EntityCode.ShouldBe("Ana birim-3");
    }

    [Fact]
    public async Task Should_Not_Create_Zero_Units()
    {
        var exception = await Assert.ThrowsAsync<UnitGroupMustAtLeastOneUnitException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var unitGroup = await UnitGroupManager.CreateAsync("Ana birim-4", null);
            });
        });
    }

    [Fact]
    public async Task Should_Not_Create_More_Than_One_Main_Unit()
    {
        var exception = await Assert.ThrowsAsync<UnitGroupMustOneMainUnitException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var unitGroup = await UnitGroupManager.CreateAsync(
                    "Ana birim-4",
                    new List<Unit>()
                    {
                        new Unit(
                            "Alt birim-1",
                            1,
                            1,
                            true,
                            mainUnit: true),

                        new Unit(
                            "Alt birim-2",
                            1,
                            1,
                            true,
                            mainUnit: true)
                    });
            });
        });
    }

    [Fact]
    public async Task Should_Not_Create_Existing_Unit_Code()
    {
        var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var unitGroup = await UnitGroupManager.CreateAsync(
                    "Ana birim-4",
                    new List<Unit>()
                    {
                        new Unit(
                            "Alt birim-1",
                            1,
                            1,
                            false,
                            mainUnit: true),

                        new Unit(
                            "Alt birim-1",
                            1,
                            1.5m,
                            true)
                    });
            });
        });

        exception.EntityCode.ShouldBe("Alt birim-1");
    }

    [Fact]
    public async Task Should_Update_A_Valid_UnitGroup()
    {
        UnitGroup unitGroup;

        await WithUnitOfWorkAsync(async () =>
        {
            unitGroup = await UnitGroupRepository.GetAsync(1);
            await UnitGroupManager.ChangeCodeAsync(unitGroup, "Ana birim-4");
            await UnitGroupManager.UpdateUnitsAsync(
                unitGroup,
                new List<Unit>()
                {
                    new Unit(
                        "Alt birim-1",
                        1,
                        1,
                        true,
                        mainUnit: true,
                        id: 1),

                    new Unit(
                        "Alt birim-2",
                        1,
                        5.5m,
                        true,
                        id: 2),

                    new Unit(
                        "Alt birim-5",
                        1,
                        5.5m,
                        true),
                });

            await UnitGroupRepository.UpdateAsync(unitGroup);
        });

        unitGroup = await UnitGroupRepository.GetAsync(1);

        unitGroup.Code.ShouldBe("Ana birim-4");
        unitGroup.Units.Count.ShouldBe(3);
        Assert.Contains(unitGroup.Units, x => x.Code == "Alt birim-5");
    }

    [Fact]
    public async Task Should_Not_Update_Existing_UnitGroup_Code()
    {
        var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UnitGroup unitGroup = await UnitGroupRepository.GetAsync(1);
                await UnitGroupManager.ChangeCodeAsync(unitGroup, "Ana birim-3");
            });
        });

        exception.EntityCode.ShouldBe("Ana birim-3");
    }

    [Fact]
    public async Task Should_Not_Update_Zero_Units()
    {
        var exception = await Assert.ThrowsAsync<UnitGroupMustAtLeastOneUnitException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UnitGroup unitGroup = await UnitGroupRepository.GetAsync(1);
                await UnitGroupManager.UpdateUnitsAsync(unitGroup, null);
            });
        });
    }

    [Fact]
    public async Task Should_Not_Update_More_Than_One_Main_Unit()
    {
        var exception = await Assert.ThrowsAsync<UnitGroupMustOneMainUnitException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UnitGroup unitGroup = await UnitGroupRepository.GetAsync(1);
                await UnitGroupManager.UpdateUnitsAsync(
                    unitGroup,
                    new List<Unit>()
                    {
                        new Unit(
                            "Alt birim-1",
                            1,
                            1,
                            true,
                            mainUnit: true),

                        new Unit(
                            "Alt birim-2",
                            1,
                            1,
                            true,
                            mainUnit: true)
                    });
            });
        });
    }

    [Fact]
    public async Task Should_Not_Update_Existing_Unit_Code()
    {
        var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UnitGroup unitGroup = await UnitGroupRepository.GetAsync(2);
                await UnitGroupManager.UpdateUnitsAsync(
                    unitGroup,
                    new List<Unit>()
                    {
                        new Unit(
                            "Alt birim-1",
                            1,
                            1,
                            false,
                            mainUnit: true),

                        new Unit(
                            "Alt birim-1",
                            1,
                            1.5m,
                            true)
                    });
            });
        });

        exception.EntityCode.ShouldBe("Alt birim-1");
    }
}
