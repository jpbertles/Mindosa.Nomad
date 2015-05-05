using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    /// <summary>
    /// This should be able to handle most data sources, but I may explicitly add additional servers as appropriate
    /// </summary>
    public class OdbcMigrationRepository: IMigrationRepository
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
