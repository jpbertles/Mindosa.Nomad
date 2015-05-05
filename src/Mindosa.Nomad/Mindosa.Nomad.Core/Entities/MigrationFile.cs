using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Entities
{
    public class MigrationFile
    {
        public MigrationFileType MigrationFileType { get; set; }
        public MigrationVersion MigrationVersion { get; set; }
        public MigrationVersion BeginningMigrationVersion { get; set; }
        public MigrationVersion EndingMigrationVersion { get; set; }
        
        public string Description { get; set; }

        public ScriptLocation ScriptLocation { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", MigrationVersion, Description, MigrationFileType.ToString());
        }
    }
}
