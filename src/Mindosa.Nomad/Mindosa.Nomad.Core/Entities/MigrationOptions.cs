using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Entities
{
    public class MigrationOptions
    {
        public MigrationCommand Command { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
        public ScriptLocation[] ScriptLocations { get; set; }
        public bool ValidateHashCodes { get; set; }
        public bool Verbose { get; set; }
    }
}
