using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindosa.Nomad.Core;
using Mindosa.Nomad.Core.Entities;

namespace Mindosa.Nomad.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Nomad a Mindosa Project");
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();

            var migrationManager = new MigrationManager();

            migrationManager.MigrationFilesLoaded += migrationManager_MigrationFilesLoaded;
            
            migrationManager.PreMigrationBeginning += migrationManager_PreMigrationBeginning;
            migrationManager.PreMigrationEnded += migrationManager_PreMigrationEnded;
            
            migrationManager.PostMigrationBeginning += migrationManager_PostMigrationBeginning;
            migrationManager.PostMigrationEnding += migrationManager_PostMigrationEnding;

            var migrationOptions = new MigrationOptions()
            {
                ConnectionString = @"server=.\sql2014;database=Nomad;Integrated Security = true;",
                ProviderName = "SqlServer",
                ScriptLocations = new ScriptLocation[]
                {
                    new ScriptLocation()
                    {
                        FullFileName = @"D:\Code\Git\Mindosa.Nomad\src\Mindosa.Nomad\Mindosa.Nomad.Core.Tests\TestScripts",
                        LocationType = ScriptLocationType.FileSystem
                    }
                }
            };

            System.Console.WriteLine("Executing baseline");
            migrationManager.Execute(MigrationCommand.Baseline, migrationOptions);

            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("Executing migrate");
            migrationManager.Execute(MigrationCommand.Migrate, migrationOptions);

            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        static void migrationManager_PostMigrationEnding(MigrationFile migrationFile, CancelEventArgs cancelEventArgs)
        {
            System.Console.WriteLine("ending post " + migrationFile.ToString());
        }

        static void migrationManager_PostMigrationBeginning(MigrationFile migrationFile, CancelEventArgs cancelEventArgs)
        {
            System.Console.WriteLine("beginning post " + migrationFile.ToString());
        }

        static void migrationManager_PreMigrationEnded(MigrationFile migrationFile, CancelEventArgs cancelEventArgs)
        {
            System.Console.WriteLine("ending pre " + migrationFile.ToString());
        }

        static void migrationManager_PreMigrationBeginning(MigrationFile migrationFile, CancelEventArgs cancelEventArgs)
        {
            System.Console.WriteLine("beginning pre " + migrationFile.ToString());
        }

        static void migrationManager_MigrationFilesLoaded(List<MigrationFile> loadedMigrationFiles, CancelEventArgs cancelEventArgs)
        {
            System.Console.WriteLine(loadedMigrationFiles.Count + " migration files loaded");

            cancelEventArgs.Cancel = true;
        }
    }
}
