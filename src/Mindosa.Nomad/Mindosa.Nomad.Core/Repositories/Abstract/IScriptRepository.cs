using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;

namespace Mindosa.Nomad.Core.Repositories.Abstract
{
    public interface IScriptRepository
    {
        IList<MigrationFile> GetFilesInPath(string path);
        string ReadFile(MigrationFile file);
    }
}
