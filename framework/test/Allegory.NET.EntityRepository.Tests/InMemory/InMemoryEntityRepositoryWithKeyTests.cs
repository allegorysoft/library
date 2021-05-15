using Allegory.NET.EntityRepository.Tests.Setup.Entities;
using Allegory.Standart.EntityRepository.Abstract;
using Allegory.Standart.EntityRepository.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allegory.NET.EntityRepository.Tests.InMemory
{
    [TestClass]
    public class InMemoryEntityRepositoryWithKeyTests
    {
        private static IEntityRepository<Table1, int> EntityRepository { get; set; }
        [ClassInitialize]
        public static void TestInitialize(TestContext testContexts) => EntityRepository = new InMemoryEntityRepository<Table1, int>();

        [TestMethod]
        public void GetById()
        {
            Table1 record = new Table1
            {
                CustomField1 = "getRecord",
                CustomField2 = 12,
                Id=13401
            };

            EntityRepository.Add(record);
            Table1 result = EntityRepository.GetById(record.Id);

            Assert.AreEqual(record.CustomField1, result.CustomField1);
        }

        [TestMethod]
        public void AddOrUpdate()
        {
            Table1 record = new Table1
            {
                CustomField1 = "data1"
            };

            EntityRepository.AddOrUpdate(record);
            record.CustomField2 = 100;
            EntityRepository.Update(record);

            Assert.IsNotNull(record.ModifiedDate);
        }

        [TestMethod]
        public void DeleteById()
        {
            Table1 record = new Table1
            {
                CustomField1 = "deletedRecord",
                Id=53123
            };

            var addedRecord = EntityRepository.Add(record);
            EntityRepository.DeleteById(record.Id);
            Table1 getById = EntityRepository.Get(f => f.Id == record.Id);

            Assert.IsNull(getById);
        }

        [TestMethod, Description("Default paged list by id")]
        public void GetPagedList()
        {
            for (int i = 1; i <= 6; i++)
                AddOrUpdate();

            var pagedList = EntityRepository.GetPagedList(pageSize: 5);

            Assert.AreEqual(5, pagedList.Results.Count);
            Assert.IsTrue(pagedList.PageCount > 1);
        }
    }
}
