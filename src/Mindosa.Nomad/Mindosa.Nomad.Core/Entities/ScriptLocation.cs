using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Entities
{
    public class ScriptLocation
    {
        public string FullFileName { get; set; }
        public ScriptLocationType LocationType { get; set; }
    }
}
