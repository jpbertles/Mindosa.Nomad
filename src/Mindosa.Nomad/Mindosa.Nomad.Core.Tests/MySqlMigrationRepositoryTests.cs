using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Repositories.Concrete;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Mindosa.Nomad.Core.Tests
{
    [TestFixture]
    public class MySqlMigrationRepositoryTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            try
            {
                using (
                    var connection =
                        new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlNoBaseline"].ConnectionString))
                {
                    var command = connection.CreateCommand();
                    connection.Open();

                    command.CommandText = "drop table if exists nomad_history";
                    command.ExecuteNonQuery();

                    command.CommandText = "drop table if exists testtable";
                    command.ExecuteNonQuery();

                    command.CommandText = "drop procedure if exists TestTable_Select_ById";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        [Test]
        [ExpectedException(typeof(MySqlException))]
        public void V01_Test_Get_Info_Where_Not_Exists()
        {
            // arrange
            var repository = new MySqlMigrationRepository(ConfigurationManager.ConnectionStrings["MySqlNoBaseline"].ConnectionString);

            // act
            var history = repository.GetInfo();

            // assert
            Assert.IsNotNull(history);
            Assert.IsEmpty(history);
        }

        [Test]
        public void V02_Test_Set_Baseline_And_Getting_Info()
        {
            // arrange
            var repository = new MySqlMigrationRepository(ConfigurationManager.ConnectionStrings["MySqlNoBaseline"].ConnectionString);
            repository.SetBaseline();

            // act
            var history = repository.GetInfo();

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
            var repository = new MySqlMigrationRepository(ConfigurationManager.ConnectionStrings["MySqlNoBaseline"].ConnectionString);
            var scriptRepository = new EmbeddedResourceScriptRepository();

            // act
            var files = scriptRepository.GetFilesInPath("Mindosa.Nomad.Core.Tests.MySqlScripts");
            repository.ApplyMigration(files[0]);

            var history = repository.GetInfo();

            // assert
            Assert.IsNotNull(history);
            Assert.IsNotEmpty(history);
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(MigrationStatus.Baseline, history[0].Status);
            Assert.AreEqual(MigrationStatus.Deployed, history[1].Status);
        }
    }
}
