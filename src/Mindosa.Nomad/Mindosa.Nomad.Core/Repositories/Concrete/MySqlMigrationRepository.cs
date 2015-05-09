using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Repositories.Abstract;
using MySql.Data.MySqlClient;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class MySqlMigrationRepository: MigrationRepositoryBase
    {
        public MySqlMigrationRepository(string connectionString) : base(connectionString, "MySql")
        {
        }

        protected override IDbConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        protected override string[] GetCommandText(string fullScript)
        {
            return new string[] {fullScript};
        }

        protected override void AddParameter(IDbCommand command, string parameterName, object value)
        {
            ((MySqlCommand) command).Parameters.AddWithValue(parameterName, value);
        }
    }
}
