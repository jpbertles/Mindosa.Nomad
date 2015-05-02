using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Repositories.Concrete;
using NUnit.Framework;

namespace Mindosa.Nomad.Core.Tests
{
    [TestFixture]
    public class EmbeddedResourceScriptRepositoryTests
    {
        [Test]
        public void Unfound_Directory_Does_Not_Exception()
        {
            // Arrange
            var repository = new EmbeddedResourceScriptRepository();

            // Act
            var files = repository.GetFilesInPath(Guid.NewGuid().ToString());

            // Assert
            Assert.IsEmpty(files);
        }

        [Test]
        public void Empty_Directory_Returns_No_Results()
        {
            // Arrange
            var path = @"c:\temp\" + Guid.NewGuid();
            var repository = new EmbeddedResourceScriptRepository();

            // Act
            var files = repository.GetFilesInPath("Mindosa.Nomad.Core.Tests.TestScripts.Empty");

            // Assert
            Assert.IsEmpty(files);
        }

        [Test]
        public void Created_Directory_Returns_Single_Result()
        {
            // Arrange
            var repository = new EmbeddedResourceScriptRepository();

            // Act
            var files = repository.GetFilesInPath("Mindosa.Nomad.Core.Tests.TestScripts.SubFolder");
            
            // Assert
            Assert.IsNotEmpty(files);
            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(MigrationFileType.Migration, files[0].MigrationFileType);
            Assert.AreEqual("Test Nested File", files[0].Description);
            Assert.AreEqual("2", files[0].MigrationVersion.GetVersion());
        }

        [Test]
        public void Nested_Directory_Returns_Expected_Results()
        {
            // Arrange
            var repository = new EmbeddedResourceScriptRepository();
           
            // Act
            var files = repository.GetFilesInPath("Mindosa.Nomad.Core.Tests.TestScripts");

            // Assert
            Assert.IsNotEmpty(files);
            Assert.AreEqual(2, files.Count);
           
            Assert.AreEqual(MigrationFileType.Migration, files[0].MigrationFileType);
            Assert.AreEqual("Test File", files[0].Description);
            Assert.AreEqual("1", files[0].MigrationVersion.GetVersion());

            Assert.AreEqual(MigrationFileType.Migration, files[1].MigrationFileType);
            Assert.AreEqual("Test Nested File", files[1].Description);
            Assert.AreEqual("2", files[1].MigrationVersion.GetVersion());
        }
    }
}
