using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Mindosa.Nomad.Console
{
    public class CommandLineOptions
    {
        [Option('n',"connection", HelpText = "ADO.Net connection string")]
        public string ConnectionString { get; set; }

        [Option('p', "provider", HelpText = "Provider name")]
        public string ProviderName { get; set; }

        [Option("validate", HelpText = "Validate script hash codes", DefaultValue = true)]
        public bool ValidateHashCodes { get; set; }

        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('l', "location", HelpText = "Script location")]
        public string ScriptLocation { get; set; }

        [Option('t', "type", HelpText = "Location type")]
        public string LocationType { get; set; }

        [Option('c', "command", HelpText = "Command name")]
        public string CommandName { get; set; }

        [Option("config", HelpText = "Configuration path")]
        public string ConfigPath { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
