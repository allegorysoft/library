using System;
using Allegory.NET.EntityRepository.Tests.Setup.Entities;
using Allegory.Standart.EntityRepository.Abstract;
using Allegory.Standart.EntityRepository.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allegory.NET.EntityRepository.Tests.InMemory
{
    [TestClass]
    public class InMemoryEntityRepositoryTests
    {
        private static IEntityRepository<Table1> EntityRepository { get; set; }
        [ClassInitialize]
        public static void TestInitialize(TestContext testContexts) => EntityRepository = new InMemoryEntityRepository<Table1>();

        [TestMethod, Description("When record added auto fill created date")]
        public void Add()
        {
            Table1 record = new Table1
            {
                CustomField1 = "data1",
                CustomField2 = 10.5,

                ModifiedDate = DateTime.Now
            };

            var addedRecord = EntityRepository.Add(record);

            Assert.AreSame(record, addedRecord);
            Assert.IsNull(record.ModifiedDate);
            Assert.AreNotEqual(DateTime.MinValue, record.CreatedDate);
        }

        [TestMethod, Description("Get record by expression")]
        public void Get()
        {
            Table1 record = new Table1
            {
                CustomField1 = Guid.NewGuid().ToString(),
                CustomField2 = 12,
            };

            EntityRepository.Add(record);
            Table1 result = EntityRepository.Get(f => f.CustomField1 == record.CustomField1);

            Assert.AreEqual(record.CustomField2, result.CustomField2);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void GetSingle()
        {
            Table1 record = new Table1
            {
                CustomField1 = "getRecord",
                CustomField2 = 12,
            };
            Table1 record2 = new Table1
            {
                CustomField1 = "getRecord",
                CustomField2 = 12,
            };

            EntityRepository.Add(record);
            EntityRepository.Add(record2);
            Table1 result = EntityRepository.GetSingle(f => f.CustomField1 == "getRecord");
        }

        [TestMethod, Description("When record updated auto fill modified date")]
        public void Update()
        {
            Table1 record = new Table1
            {
                CustomField1 = "addedRecord",

                ModifiedDate = DateTime.Now,
                ModifiedBy = 15
            };

            EntityRepository.Add(record);
            Assert.IsNull(record.ModifiedDate);

            record.CustomField1 = "updatedRecord";
            record.ModifiedBy = 15;
            EntityRepository.Update(record);

            Assert.IsNotNull(record.ModifiedDate);
        }

        [TestMethod]
        public void Delete()
        {
            Table1 record = new Table1
            {
                CustomField1 = "deletedRecord"
            };

            var addedRecord = EntityRepository.Add(record);
            EntityRepository.Delete(record);
            Table1 getById = EntityRepository.Get(f => f.CustomField1 == record.CustomField1);

            Assert.IsNull(getById);
        }

        [TestMethod]
        public void GetList()
        {
            Add();

            var result = EntityRepository.GetList();

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetPagedList()
        {
            for (int i = 1; i <= 6; i++)
                Add();

            var pagedList = EntityRepository.GetPagedList(o => o.Id, pageSize: 5);

            Assert.AreEqual(5, pagedList.Results.Count);
            Assert.IsTrue(pagedList.PageCount > 1);
        }
    }
}
