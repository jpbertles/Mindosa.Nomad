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
    public class FileSystemScriptRepositoryTests
    {
        [Test]
        [ExpectedException(typeof (DirectoryNotFoundException))]
        public void Unfound_Directory_Throws_Exception()
        {
            // Arrange
            var repository = new FileSystemScriptRepository();

            // Act
            var files = repository.GetFilesInPath(@"c:\" + Guid.NewGuid());

            // No assertion...there will be an exception
        }

        [Test]
        public void Empty_Directory_Returns_No_Results()
        {
            // Arrange
            var path = @"c:\temp\" + Guid.NewGuid();
            var repository = new FileSystemScriptRepository();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Act
            var files = repository.GetFilesInPath(path);

            // Clean up
            Directory.Delete(path);

            // Assert
            Assert.IsEmpty(files);
        }

        [Test]
        public void Created_Directory_Returns_Single_Result()
        {
            // Arrange
            var path = @"c:\temp\" + Guid.NewGuid();
            var repository = new FileSystemScriptRepository();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "1__Test_File.migration.sql"), string.Empty);

            // Act
            var files = repository.GetFilesInPath(path);

            // Clean up
            Directory.Delete(path,true);

            // Assert
            Assert.IsNotEmpty(files);
            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(MigrationFileType.Migration, files[0].MigrationFileType);
            Assert.AreEqual("Test File", files[0].Description);
            Assert.AreEqual("1", files[0].MigrationVersion.GetVersion());
            Assert.AreEqual(ScriptLocationType.FileSystem, files[0].ScriptLocation.LocationType);
        }

        [Test]
        public void Nested_Directory_Returns_Expected_Results()
        {
            // Arrange
            var path = @"c:\temp\" + Guid.NewGuid();
            var repository = new FileSystemScriptRepository();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "1__Test_File.migration.sql"), string.Empty);

            var nestedPath = Path.Combine(path, Guid.NewGuid().ToString());
            if (!Directory.Exists(nestedPath))
            {
                Directory.CreateDirectory(nestedPath);
            }
            File.WriteAllText(Path.Combine(nestedPath, "2__Test_Nested_File.migration.sql"), string.Empty);


            // Act
            var files = repository.GetFilesInPath(path);

            // Clean up
            Directory.Delete(path, true);

            // Assert
            Assert.IsNotEmpty(files);
            Assert.AreEqual(2, files.Count);
           
            Assert.AreEqual(MigrationFileType.Migration, files[0].MigrationFileType);
            Assert.AreEqual("Test File", files[0].Description);
            Assert.AreEqual("1", files[0].MigrationVersion.GetVersion());
            Assert.AreEqual(ScriptLocationType.FileSystem, files[0].ScriptLocation.LocationType);

            Assert.AreEqual(MigrationFileType.Migration, files[1].MigrationFileType);
            Assert.AreEqual("Test Nested File", files[1].Description);
            Assert.AreEqual("2", files[1].MigrationVersion.GetVersion());
            Assert.AreEqual(ScriptLocationType.FileSystem, files[1].ScriptLocation.LocationType);
        }

        [Test]
        public void Created_Directory_Reads_Single_Result()
        {
            // Arrange
            var content = Guid.NewGuid().ToString();
            var path = @"c:\temp\" + Guid.NewGuid();
            var repository = new FileSystemScriptRepository();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "1__Test_File.migration.sql"), content);

            // Act
            var files = repository.GetFilesInPath(path);
            var retrievedContent = repository.ReadFile(files[0]);

            // Clean up
            Directory.Delete(path, true);

            // Assert
            Assert.AreEqual(content, retrievedContent);
        }
    }
}
