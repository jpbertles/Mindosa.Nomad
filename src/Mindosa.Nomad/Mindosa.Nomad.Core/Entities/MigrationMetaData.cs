using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Entities
{
    public class MigrationMetaData
    {
        public int MigrationId { get; set; }
        public string MigrationVersion { get; set; }
        public string Description { get; set; }
        public MigrationStatus Status { get; set; }
        public int HashCode { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
