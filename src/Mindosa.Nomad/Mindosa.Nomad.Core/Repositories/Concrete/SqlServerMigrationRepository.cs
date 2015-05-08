using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Abstract;

namespace Mindosa.Nomad.Core.Repositories.Concrete
{
    public class SqlServerMigrationRepository: IMigrationRepository
    {
        public List<Entities.MigrationMetaData> GetInfo(string connectionString)
        {
            var history = new List<MigrationMetaData>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "select HashCode, Description, MigrationVersion, [Status], TimeStamp, MigrationId from Nomad_History";
                
                connection.Open();
                try
                {
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
                catch (SqlException ex)
                {
                    // if there is no history, so be it....we're still going to return an empty result set
                }
            }

            return history;
        }

        public void ApplyMigration(Entities.MigrationFile migrationFile, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var transaction = new TransactionScope())
                {
                    var command = connection.CreateCommand();
                    connection.Open();

                    //TODO: we will need to split the go statements here because we don't want to require SMO
                    command.CommandText = MigrationFileFactory.GetContents(migrationFile);
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO dbo.Nomad_History (MigrationVersion, [Description], [Status], HashCode, [TimeStamp]) values (@MigrationVersion, @Description, @Status, @HashCode, getutcdate())";
                    command.Parameters.AddWithValue("@MigrationVersion", migrationFile.MigrationVersion.GetVersion());
                    command.Parameters.AddWithValue("@Description", migrationFile.Description);
                    command.Parameters.AddWithValue("@Status", MigrationStatus.Deployed);
                    command.Parameters.AddWithValue("@HashCode", migrationFile.GetHashCode());

                    command.ExecuteNonQuery();

                    transaction.Complete();
                }
            }
        }

        public void SetBaseline(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = @"
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Nomad_History')
BEGIN
    CREATE TABLE dbo.Nomad_History (MigrationId int not null primary key identity, MigrationVersion varchar(max) not null, [Description] varchar(max) not null, [Status] varchar(50) not null, HashCode int, [TimeStamp] DateTime not null)
END
";
                command.ExecuteNonQuery();



                command.CommandText = @"
IF NOT EXISTS (SELECT * FROM dbo.Nomad_History)
BEGIN
    INSERT INTO dbo.Nomad_History (MigrationVersion, [Description], [Status], HashCode, [TimeStamp]) values ('0', 'Baseline', 'Baseline', " + string.Empty.GetHashCode() + @", getutcdate())
END
";
                command.ExecuteNonQuery();
            }
        }
    }
}
