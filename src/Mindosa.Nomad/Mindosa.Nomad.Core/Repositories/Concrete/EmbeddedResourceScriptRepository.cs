using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class EmbeddedResourceScriptRepository: IScriptRepository
    {
        public IList<MigrationFile> GetFilesInPath(string path)
        {
            var migrationFiles = new List<MigrationFile>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
                .Where(
                    assembly =>
                        assembly.GetManifestResourceNames()
                            .Any(x => x.StartsWith(path, StringComparison.CurrentCultureIgnoreCase))))
            {
                foreach (
                    var manifestResourceName in
                        assembly.GetManifestResourceNames()
                            .Where(x => x.StartsWith(path, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var fileName = manifestResourceName.Replace(path + ".", string.Empty);

                    var fileNameParts = fileName.Split('.');
                    if (fileNameParts.Length > 3)
                    {
                        fileName = string.Join(".", fileNameParts.SkipWhile(x => !char.IsNumber(x[0])));
                    }

                    migrationFiles.Add(MigrationFileFactory.Create(manifestResourceName, fileName,
                        ScriptLocationType.EmbeddedResource));
                }
            }

            return migrationFiles.OrderBy(migrationFile => migrationFile.MigrationVersion).ToList();
        }

        public string ReadFile(MigrationFile file)
        {
            return ReadFile(file.ScriptLocation.FullFileName);
        }

        internal string ReadFile(string resourceName)
        {
            var tmpStuff = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().ToArray();

            var fileAssembly =
                AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(
                        assembly =>
                            assembly.GetManifestResourceNames()
                                .Any(resource => resource.Equals(resourceName, StringComparison.CurrentCultureIgnoreCase)));

            using (var sr = new StreamReader(fileAssembly.GetManifestResourceStream(resourceName)))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
