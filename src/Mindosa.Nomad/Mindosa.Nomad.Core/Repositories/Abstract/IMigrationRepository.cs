using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;

namespace Mindosa.Nomad.Core.Repositories.Abstract
{
    public interface IMigrationRepository
    {
        List<Entities.MigrationMetaData> GetInfo();
        void SetBaseline();
        void ApplyMigration(Entities.MigrationFile migrationFile);
    }
}
