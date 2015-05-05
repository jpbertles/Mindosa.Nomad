using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class FileSystemScriptRepository: IScriptRepository
    {
        public IList<MigrationFile> GetFilesInPath(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.EnumerateFiles("*.sql", SearchOption.AllDirectories)
                    .Select(fileInfo => MigrationFileFactory.Create(fileInfo.FullName, fileInfo.Name, ScriptLocationType.FileSystem))
                    .OrderBy(migrationFile => migrationFile.MigrationVersion)
                    .ToList();
        }

        public string ReadFile(MigrationFile file)
        {
            return File.ReadAllText(file.ScriptLocation.FullFileName);
        }
    }
}
