using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Concrete;

namespace Mindosa.Nomad.Core.Repositories.Abstract
{
    public abstract class MigrationRepositoryBase : IMigrationRepository
    {
        protected readonly string ConnectionString;
        protected readonly string ProviderType;
        protected readonly EmbeddedResourceScriptRepository ScriptRepository;

        protected MigrationRepositoryBase(string connectionString, string providerType)
        {
            ConnectionString = connectionString;
            ProviderType = providerType;

            ScriptRepository = new EmbeddedResourceScriptRepository();
        }

        protected abstract IDbConnection GetConnection();
        protected abstract string[] GetCommandText(string fullScript);
        protected abstract void AddParameter(IDbCommand command, string parameterName, object value);

        protected string GetResourceName(string scriptName)
        {
            return "Mindosa.Nomad.Core.EmbeddedScripts." + ProviderType + "." + scriptName + ".sql";
        }

        public List<Entities.MigrationMetaData> GetInfo()
        {
            var history = new List<MigrationMetaData>();

            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = ScriptRepository.ReadFile(GetResourceName("GetInfo"));

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var migrationMetaData = new MigrationMetaData
                        {
                            HashCode = reader.GetInt32(0),
                            Description = reader.GetString(1),
                            MigrationVersion = reader.GetString(2),
                            Status = (MigrationStatus) Enum.Parse(typeof (MigrationStatus), reader.GetString(3)),
                            TimeStamp = reader.GetDateTime(4),
                            MigrationId = reader.GetInt32(5)
                        };

                        history.Add(migrationMetaData);
                    }
                }
            }

            return history;
        }

        public void SetBaseline()
        {
            using (var connection = GetConnection())
            {
                var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = ScriptRepository.ReadFile(GetResourceName("CreateHistory"));
                command.ExecuteNonQuery();

                var baseline = ScriptRepository.ReadFile(GetResourceName("SetBaseline"));

                command.CommandText = string.Format(baseline, baseline.GetHashCode());
                command.ExecuteNonQuery();
            }
        }

        public void ApplyMigration(Entities.MigrationFile migrationFile)
        {
            using (var connection = GetConnection())
            {
                using (var transaction = new TransactionScope())
                {
                    var command = connection.CreateCommand();
                    connection.Open();

                    // in reality, this is so we can execute script statements for SQL Server
                    // where different statements are seperated by GO, which you can't use
                    // without a dependency on SMO...so splitting the script ensues
                    var migrationScript = MigrationFileFactory.GetContents(migrationFile);
                    foreach (var commandText in GetCommandText(migrationScript))
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = ScriptRepository.ReadFile(GetResourceName("InsertScriptHistory"));
                    AddParameter(command, "@MigrationVersion", migrationFile.MigrationVersion.GetVersion());
                    AddParameter(command, "@Description", migrationFile.Description);
                    AddParameter(command, "@Status", MigrationStatus.Deployed);
                    AddParameter(command, "@HashCode", migrationFile.GetHashCode());

                    command.ExecuteNonQuery();

                    transaction.Complete();
                }
            }
        }
    }
}
