using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Repositories.Concrete;
using NUnit.Framework;

namespace Mindosa.Nomad.Core.Tests
{
    [TestFixture]
    public class MigrationVersionTests
    {
        #region data sources
        private static string[] InvalidVersions =
        {
            "a",
            "1a",
            "a1",
            "1-1",
            "1:1",
            "",
            "~"
        };

        private static string[] ValidVersions =
        {
            "1",
            "1.1",
            "1.1.1",
            "1.1.1.1.1.1.1.1.1.1.1.1",
            "2015.05.03.09.52",
            "1_1",
            "1_1_1",
            "1_1_1_1_1_1_1_1_1_1_1_1",
            "2015_05_03_09_52"
        };

        public static object[] ComparisonVersions =
        {
            new object [] { "2", "1" },
            new object [] { "1.2.3", "1.2.2" },
            new object [] { "1.2.3", "1.2" },
            new object [] { "2", "1.99999" }
        };
        #endregion

        [Test]
        [ExpectedException(typeof(VersionNotFoundException))]
        [TestCaseSource("InvalidVersions")]
        public void Invalid_Version_Throws_Exception(string version)
        {
            var migrationVersion = MigrationVersion.FromVersion(version);
        }

        [Test]
        [TestCaseSource("ValidVersions")]
        public void Valid_Versions_Parse(string version)
        {
            var migrationVersion = MigrationVersion.FromVersion(version);

            Assert.IsNotNull(migrationVersion);
            Assert.AreNotEqual(MigrationVersion.EMPTY, migrationVersion);
            Assert.AreNotEqual(MigrationVersion.LATEST, migrationVersion);
            Assert.AreNotEqual(MigrationVersion.CURRENT, migrationVersion);
        }

        [Test]
        [TestCaseSource("ComparisonVersions")]
        public void Version_Comparison_Version1_Always_Greater_Than_Version2(string version1, string version2)
        {
            var versions = new List<MigrationVersion>()
            {
                MigrationVersion.FromVersion(version1),
                MigrationVersion.FromVersion(version2)
            };

            versions.Sort();

            Assert.IsTrue(versions[1].GetVersion().Equals(version1));
        }
    }
}
