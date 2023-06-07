using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace Allegory.Saler.Units
{
    public class UnitGroupAppService_Tests : SalerApplicationTestBase
    {
        protected IUnitGroupAppService UnitGroupAppService { get; }

        public UnitGroupAppService_Tests()
        {
            UnitGroupAppService = GetRequiredService<IUnitGroupAppService>();
        }

        [Fact]
        public async Task Should_Get_List_Of_UnitGroups()
        {
            //Act
            var result = await UnitGroupAppService.ListAsync(new FilteredPagedAndSortedResultRequestDto());

            //Assert
            result.TotalCount.ShouldBeGreaterThan(0);
            result.Items.ShouldContain(b => b.Code == "Ana birim-1");
        }

        [Fact]
        public async Task Should_Create_A_Valid_UnitGroup()
        {
            //Act
            var result = await UnitGroupAppService.CreateAsync(
                new UnitGroupCreateDto
                {
                    Code = "Ana birim-4",
                    Name = "Ana birim-4",
                    Units = new[]
                   {
                       new UnitCreateDto
                       {
                           Code = "Alt birim-1",
                           Name = "Alt birim-1",
                           ConvFact1 = 1,
                           ConvFact2 = 1,
                           MainUnit = true
                       },
                       new UnitCreateDto
                       {
                           Code = "Alt birim-2",
                           Name = "Alt birim-2",
                           ConvFact1 = 1,
                           ConvFact2 = 1,
                           MainUnit = false
                       }
                   }
                }
            );

            //Assert
            result.Id.ShouldNotBe(0);
            result.Code.ShouldBe("Ana birim-4");
        }

        [Fact]
        public async Task Should_Not_Create_Existing_UnitGroup_Code()
        {
            var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-3",
                         Name = "Ana birim-3",
                         Units = new List<UnitCreateDto>()
                     }
                );
            });

            exception.EntityCode.ShouldBe("Ana birim-3");
        }

        [Fact]
        public async Task Should_Not_Create_Zero_Units()
        {
            var exception = await Assert.ThrowsAsync<UnitGroupMustAtLeastOneUnitException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                     }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Create_Existing_Unit_Code()
        {
            var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                         {
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 ConvFact1 = 1,
                                 ConvFact2 = 1,
                                 MainUnit = true
                             },
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 ConvFact1 = 1,
                                 ConvFact2 = 2,
                             }
                         }
                     }
                );
            });

            exception.EntityCode.ShouldBe("Alt birim-1");
        }

        [Fact]
        public async Task Should_Not_Create_More_Than_One_Main_Unit()
        {
            var exception = await Assert.ThrowsAsync<UnitGroupMustOneMainUnitException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                         {
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 ConvFact1 = 1,
                                 ConvFact2 = 1,
                                 MainUnit = true
                             },
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-2",
                                 Name = "alt birim-2",
                                 ConvFact1 = 1,
                                 ConvFact2 = 1,
                                 MainUnit = true
                             }
                         }
                     }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Create_When_Main_Unit_Conv_Fact_Not_Equals_One()
        {
            var exception = await Assert.ThrowsAsync<MainUnitConvFactMustOneException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                         {
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 MainUnit = true,
                                 ConvFact1 = 1,
                                 ConvFact2 = 2
                             },
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-2",
                                 Name = "alt birim-2"
                             }
                         }
                     }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Create_When_Unit_Conv_Fact_Equals_Zero()
        {
            var exception = await Assert.ThrowsAsync<UnitConvFactMustSetException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                         {
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 MainUnit = true,
                                 ConvFact1 = 1,
                                 ConvFact2 = 1
                             },
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-2",
                                 Name = "alt birim-2",
                                 ConvFact1 = 1,
                                 ConvFact2 = 0
                             }
                         }
                     }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Create_When_Unit_Group_Divisible_False_And_Unit_Conv_Has_Decimal_Places()
        {
            var exception = await Assert.ThrowsAsync<UnitCannotDividedException>(async () =>
            {
                await UnitGroupAppService.CreateAsync(
                     new UnitGroupCreateDto
                     {
                         Code = "Ana birim-4",
                         Name = "Ana birim-4",
                         Units = new List<UnitCreateDto>()
                         {
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-1",
                                 Name = "alt birim-1",
                                 MainUnit = true,
                                 ConvFact1 = 1,
                                 ConvFact2 = 1
                             },
                             new UnitCreateDto
                             {
                                 Code = "Alt birim-2",
                                 Name = "alt birim-2",
                                 Divisible = false,
                                 ConvFact1 = 1,
                                 ConvFact2 = 1.1m
                             }
                         }
                     }
                );
            });
        }

        [Fact]
        public async Task Should_Update_A_Valid_UnitGroup()
        {
            //Act
            var result = await UnitGroupAppService.UpdateAsync(
                1,
                new UnitGroupUpdateDto
                {
                    Code = "Ana birim-1",
                    Name = "Ana birim-1-güncel",
                    Units = new[]
                   {
                       new UnitUpdateDto
                       {
                           Id = 1,
                           Code = "Alt birim-1",
                           Name = "alt birim-1",
                           ConvFact1 = 1,
                           ConvFact2 = 1,
                           Divisible = true,
                           MainUnit = true,
                       },
                       new UnitUpdateDto
                       {
                           Id = 2,
                           Code = "Alt birim-2",
                           Name = "alt birim-2-güncel",
                           ConvFact1 = 1,
                           ConvFact2 = 20,
                           Divisible = true,
                           MainUnit = false
                       },
                       new UnitUpdateDto
                       {
                           Code = "Alt birim-5",
                           Name = "alt birim-5-yeni",
                           ConvFact1 = 1,
                           ConvFact2 = 50,
                           MainUnit = false
                       },
                   }
                }
            );

            //Assert
            result.Id.ShouldNotBe(0);
            result.Units.Count.ShouldBe(3);
            Assert.Contains(result.Units, x => x.Code == "Alt birim-5");
        }

        [Fact]
        public async Task Should_Not_Update_Existing_UnitGroup_Code()
        {
            var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-3",
                        Name = "Ana birim-3",
                        Units = new List<UnitUpdateDto>()
                    }
                );
            });

            exception.EntityCode.ShouldBe("Ana birim-3");
        }

        [Fact]
        public async Task Should_Not_Update_Zero_Units()
        {
            var exception = await Assert.ThrowsAsync<UnitGroupMustAtLeastOneUnitException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                    }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Update_Existing_Unit_Code()
        {
            var exception = await Assert.ThrowsAsync<CodeAlreadyExistsException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    2,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                ConvFact1 = 1,
                                ConvFact2 = 1,
                                MainUnit = true
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                ConvFact1 = 1,
                                ConvFact2 = 2
                            }
                        }
                    }
                );
            });

            exception.EntityCode.ShouldBe("Alt birim-1");
        }

        [Fact]
        public async Task Should_Not_Update_More_Than_One_Main_Unit()
        {
            var exception = await Assert.ThrowsAsync<UnitGroupMustOneMainUnitException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                ConvFact1 = 1,
                                ConvFact2 = 1,
                                MainUnit = true
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-2",
                                Name = "alt birim-2",
                                ConvFact1 = 1,
                                ConvFact2 = 1,
                                MainUnit = true
                            }
                        }
                    }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Update_When_Main_Unit_Conv_Fact_Not_Equals_One()
        {
            var exception = await Assert.ThrowsAsync<MainUnitConvFactMustOneException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt-1",
                                Name = "alt birim-1",
                                MainUnit = true,
                                ConvFact1 = 1,
                                ConvFact2 = 2
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-2",
                                Name = "alt birim-2"
                            }
                        }
                    }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Update_When_Unit_Conv_Fact_Equals_Zero()
        {
            var exception = await Assert.ThrowsAsync<UnitConvFactMustSetException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                MainUnit = true,
                                ConvFact1 = 1,
                                ConvFact2 = 1
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-2",
                                Name = "alt birim-2",
                                ConvFact1 = 1,
                                ConvFact2 = 0
                            }
                        }
                    }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Update_When_Unit_Group_Divisible_False_And_Unit_Conv_Has_Decimal_Places()
        {
            var exception = await Assert.ThrowsAsync<UnitCannotDividedException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto
                    {
                        Code = "Ana birim-4",
                        Name = "Ana birim-4",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                MainUnit = true,
                                ConvFact1 = 1,
                                ConvFact2 = 1
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-2",
                                Name = "alt birim-2",
                                Divisible = false,
                                ConvFact1 = 1,
                                ConvFact2 = 1.1m
                            }
                        }
                    }
                );
            });
        }

        [Fact]
        public async Task Should_Not_Delete_When_Unit_Has_Transaction_Record()
        {
            var exception = await Assert.ThrowsAsync<ThereIsTransactionRecordException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto()
                    {
                        Code = "Ana birim-1",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-10",
                                Name = "alt birim-10",
                                MainUnit = true,
                                ConvFact1 = 1,
                                ConvFact2 = 1
                            }
                        }
                    });
            });

            exception.IsDelete.ShouldBeTrue();
            exception.EntityType.ShouldBe(typeof(Unit));
            exception.TransactionEntityType.ShouldBe(typeof(Orders.Order));
        }
        
        [Fact]
        public async Task Should_Not_Update_Divisible_State_To_False_When_Unit_Has_Transaction_Record()
        {
            var exception = await Assert.ThrowsAsync<ThereIsTransactionRecordException>(async () =>
            {
                await UnitGroupAppService.UpdateAsync(
                    1,
                    new UnitGroupUpdateDto()
                    {
                        Code = "Ana birim-1",
                        Units = new List<UnitUpdateDto>()
                        {
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-1",
                                Name = "alt birim-1",
                                MainUnit = true,
                                ConvFact1 = 1,
                                ConvFact2 = 1,
                                Divisible = false,
                                Id = 1
                            },
                            new UnitUpdateDto
                            {
                                Code = "Alt birim-2",
                                Name = "alt birim-2",
                                ConvFact1 = 1,
                                ConvFact2 = 1,
                                Divisible = false,
                                Id = 2
                            }
                        }
                    });
            });

            exception.IsDelete.ShouldBeFalse();
            exception.EntityType.ShouldBe(typeof(Unit));
            exception.TransactionEntityType.ShouldBe(typeof(Orders.Order));
        }

        [Fact]
        public async Task Should_Not_Delete_Unit_Group_When_Item_Or_Service_Use_Unit_Group()
        {
            var exception = await Assert.ThrowsAsync<ThereIsTransactionRecordException>(async () =>
            {
                await UnitGroupAppService.DeleteAsync(1);
            });

            exception.EntityType.ShouldBe(typeof(UnitGroup));
        }
    }
}
