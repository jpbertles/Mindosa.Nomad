using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class SqlServerMigrationRepository: IMigrationRepository
    {
        public List<Entities.MigrationMetaData> GetInfo()
        {
            throw new NotImplementedException();
        }

        public void ApplyMigration(Entities.MigrationFile migrationFile)
        {
            throw new NotImplementedException();
        }

        public void SetBaseline()
        {
            throw new NotImplementedException();
        }
    }
}
