using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class SqlServerMigrationRepository: MigrationRepositoryBase
    {
        public SqlServerMigrationRepository(string connectionString) : base(connectionString, "SqlServer")
        {
        }
        
        protected override IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        protected override string[] GetCommandText(string fullScript)
        {
            var statements = Regex.Split(fullScript, @"^\s*GO\s* ($ | \-\- .*$)",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            return statements.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n')).ToArray();
        }

        protected override void AddParameter(IDbCommand command, string parameterName, object value)
        {
            ((SqlCommand) command).Parameters.AddWithValue(parameterName, value);
        }
    }
}
