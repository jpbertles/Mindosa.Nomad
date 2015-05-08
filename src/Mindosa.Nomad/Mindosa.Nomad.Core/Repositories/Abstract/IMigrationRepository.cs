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
        List<MigrationMetaData> GetInfo(string connectionString);
        void ApplyMigration(MigrationFile migrationFile, string connectionString);
        void SetBaseline(string connectionString);
    }
}
