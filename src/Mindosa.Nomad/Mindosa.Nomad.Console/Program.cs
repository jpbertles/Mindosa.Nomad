using System;
using System.Collections.Generic;
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
            System.Console.WriteLine("Nomad by Mindosa");
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();

            MigrationManager.Execute(MigrationCommand.Baseline, new MigrationOptions()
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
            });

            System.Console.WriteLine();
            System.Console.WriteLine();
        }
    }
}
