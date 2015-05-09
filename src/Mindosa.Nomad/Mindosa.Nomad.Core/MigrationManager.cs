﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;
using Mindosa.Nomad.Core.Infrasctructure;
using Mindosa.Nomad.Core.Repositories.Abstract;
using Mindosa.Nomad.Core.Repositories.Concrete;

namespace Mindosa.Nomad.Core
{
    public class MigrationManager
    {
        public static void Execute(MigrationCommand command, MigrationOptions options)
        {
            IMigrationRepository migrationRepository;

            switch (options.ProviderName)
            {
                case "SqlServer":
                    migrationRepository = new SqlServerMigrationRepository(options.ConnectionString);
                    break;
                case "MySql":
                    migrationRepository = new MySqlMigrationRepository(options.ConnectionString);
                    break;
                default:
                    var repositoryAssembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(assembly => assembly.GetType(options.ProviderName + "MigrationRepository", false) != null);
                    if (repositoryAssembly != null)
                    {
                        var type = repositoryAssembly.GetType(options.ProviderName + "MigrationRepository", false);
                        migrationRepository =
                            (IMigrationRepository) Activator.CreateInstance(type, options.ConnectionString);
                    }
                    else
                    {
                        throw new NotImplementedException("There is no migration repository for " + options.ProviderName);
                    }
                    break;
            }

            var migrationFiles = new List<MigrationFile>();

            foreach (var scriptLocation in options.ScriptLocations)
            {
                IScriptRepository scriptRepository;

                switch (scriptLocation.LocationType)
                {
                    case ScriptLocationType.FileSystem:
                        scriptRepository = new FileSystemScriptRepository();
                        break;
                    case ScriptLocationType.EmbeddedResource:
                        scriptRepository = new EmbeddedResourceScriptRepository();
                        break;
                    default:
                        throw  new NotImplementedException("There is no script repository for " + scriptLocation.LocationType);
                }

                migrationFiles.AddRange(scriptRepository.GetFilesInPath(scriptLocation.FullFileName));
            }

            
            var existingFiles = migrationRepository.GetInfo();
            var pendingFiles = new List<MigrationFile>();

            foreach (var migrationFile in migrationFiles)
            {
                var hashCode = MigrationFileFactory.GetContents(migrationFile).GetHashCode();
                var existingFile = existingFiles.FirstOrDefault(
                    x => MigrationVersion.FromVersion(x.MigrationVersion).Equals(migrationFile.MigrationVersion));
                if (existingFile == null)
                {
                    pendingFiles.Add(migrationFile);
                }
                else
                {
                    if (options.ValidateHashCodes && existingFile.HashCode != hashCode)
                    {
                        throw new Exception(migrationFile.ToString() + " does not match a previsouly applied migration");
                    }
                }
            }


            switch (command)
            {
                case MigrationCommand.Info:
                    break;
                case MigrationCommand.Baseline:
                    migrationRepository.SetBaseline();
                    break;
                case MigrationCommand.Migrate:
                    foreach (var migrationFile in pendingFiles)
                    {
                        migrationRepository.ApplyMigration(migrationFile);
                    }
                    break;
            }

            existingFiles = migrationRepository.GetInfo();

            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("|{0,10} | {1,30} | {2,10} | {3,17} |", "Version", "Description", "Status", "Time");
            Console.WriteLine("-------------------------------------------------------------------------------");
            foreach (var existingFile in existingFiles.OrderBy(x => MigrationVersion.FromVersion(x.MigrationVersion)))
            {
                Console.WriteLine("|{0,10} | {1,30} | {2,10} | {3,17} |", existingFile.MigrationVersion, existingFile.Description, existingFile.Status, existingFile.TimeStamp.ToLocalTime().ToString("MM-dd-yy hh:mm tt"));
                Console.WriteLine("-------------------------------------------------------------------------------");
            }

            foreach (var pendingFile in pendingFiles.OrderBy(x => x.MigrationVersion))
            {
                Console.WriteLine("|{0,10} | {1,30} | {2,10} | {3,17} |", pendingFile.MigrationVersion, pendingFile.Description, MigrationStatus.Pending, DateTime.Now.ToString("MM-dd-yy hh:mm tt"));
                Console.WriteLine("-------------------------------------------------------------------------------");
            }
        }
    }
}
