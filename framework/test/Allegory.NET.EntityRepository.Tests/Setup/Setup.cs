using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allegory.NET.EntityRepository.Tests.Setup
{
    [TestClass]
    public class Setup
    {
        const string DatabaseName = "Sample";
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            CreateDatabaseIfNotExists();
            CreateTableIfNotExists();
            SetPrincipal();
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {

        }
        private static void CreateDatabaseIfNotExists()
        {
            using (var connection = new SqlConnection("Data Source=.;Integrated security=True;"))
            {
                const string sqlCreateDatabase = @"
                IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{0}')
                BEGIN
                    CREATE DATABASE {0};
                END
                ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(string.Format(sqlCreateDatabase, DatabaseName), connection))
                    command.ExecuteNonQuery();
            }
        }
        private static void CreateTableIfNotExists()
        {
            var connection = new SqlConnection(InitConfiguration().GetConnectionString("DefaultConnection"));
            connection.Open();
            var files = new List<string>
            {
                ReadScriptFile("CreateTable1")
            };
            foreach (var setupFile in files)
            {
                using (SqlCommand command = new SqlCommand(setupFile, connection))
                    command.ExecuteNonQuery();
            }
        }
        private static string ReadScriptFile(string name)
        {
            string fileName = typeof(Setup).Namespace + ".Sql." + name + ".sql";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        private static void SetPrincipal()
        {
            ClaimsIdentity userIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,"10")
            });
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(userIdentity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }
    }
}
