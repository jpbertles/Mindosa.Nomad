using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Repositories.Concrete;
using Mindosa.Nomad.Core.Tests.Infrastructure;
using NUnit.Framework;

namespace Mindosa.Nomad.Core.Tests
{
    [TestFixture(Category = "Integration")]
    public class SqlServerMigrationRepositoryTests
    {
        private const string DatabaseName = "TestDatabase";

        [TestFixtureSetUp]
        public void Setup()
        {
            UnitTestStartAndEnd.Start(DatabaseName);
        }

        [Test]
        public void V01_Test_Get_Info_Where_Not_Exists()
        {
            // arrange
            var repository = new SqlServerMigrationRepository();

            // act
            var history =
                repository.GetInfo(ConfigurationManager.ConnectionStrings["SqlServerNoBaseline"].ConnectionString);

            // assert
            Assert.IsNotNull(history);
            Assert.IsEmpty(history);
        }

        [Test]
        public void V02_Test_Set_Baseline_And_Getting_Info()
        {
            // arrange
            var repository = new SqlServerMigrationRepository();
            repository.SetBaseline(ConfigurationManager.ConnectionStrings["SqlServerNoBaseline"].ConnectionString);
            
            // act
            var history =
                repository.GetInfo(ConfigurationManager.ConnectionStrings["SqlServerNoBaseline"].ConnectionString);

            // assert
            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(MigrationStatus.Baseline, history[0].Status);
        }

        [Test]
        public void V03_Test_Apply_Migration_And_Getting_Info()
        {
            // arrange
            var repository = new SqlServerMigrationRepository();
            var scriptRepository = new EmbeddedResourceScriptRepository();

            // act
            var files = scriptRepository.GetFilesInPath("Mindosa.Nomad.Core.Tests.TestScripts.SubFolder");
            repository.ApplyMigration(files[0], ConfigurationManager.ConnectionStrings["SqlServerNoBaseline"].ConnectionString);

            var history = repository.GetInfo(ConfigurationManager.ConnectionStrings["SqlServerNoBaseline"].ConnectionString);

            // assert
            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(MigrationStatus.Baseline, history[0].Status);
            Assert.AreEqual(MigrationStatus.Deployed, history[1].Status);
        }
    }
}
