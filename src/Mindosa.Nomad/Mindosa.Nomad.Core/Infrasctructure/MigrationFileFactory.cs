using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mindosa.Nomad.Core.Entities;

namespace Mindosa.Nomad.Core.Infrasctructure
{
    public class MigrationFileFactory
    {

        public static MigrationFile Create(string fullFileName, string fileNameWithExtension, ScriptLocationType scriptLocationType)
        {
            var migrationFileType = MigrationFileType.Migration;

            var fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);

            if (fileName.EndsWith(".migration", StringComparison.CurrentCultureIgnoreCase))
            {
                migrationFileType = MigrationFileType.Migration;
                fileName = fileName.Replace(".migration", string.Empty);
            }
            else if (fileName.EndsWith(".post"))
            {
                migrationFileType = MigrationFileType.PostMigrate;
                fileName = fileName.Replace(".post", string.Empty);
            }
            else if (fileName.EndsWith(".pre"))
            {
                migrationFileType = MigrationFileType.PostMigrate;
                fileName = fileName.Replace(".pre", string.Empty);
            }

            var nameParts = Regex.Split(fileName, "__");
            var description = nameParts.Last().Replace("_", " ");


            var migrationFile = new MigrationFile()
            {
                Description = description,
                
                MigrationFileType = migrationFileType,
                ScriptLocation = new ScriptLocation(){
                    LocationType = scriptLocationType,
                    FullFileName = fullFileName
                }
            };


            if (migrationFileType == MigrationFileType.Migration)
            {
                migrationFile.MigrationVersion = MigrationVersion.FromVersion(nameParts[0]);
            }
            else
            {
                // version number format is as follows:
                // {pre/post version/order}[-{beginningVersionNumber}[-{endingVersionNumber}]]
                var versionNumberParts = nameParts.First().Split('-');

                migrationFile.MigrationVersion = MigrationVersion.FromVersion(versionNumberParts.First());


                if (versionNumberParts.Length == 1)
                {
                    migrationFile.BeginningMigrationVersion = MigrationVersion.EMPTY;
                    migrationFile.EndingMigrationVersion = MigrationVersion.LATEST;
                }
                else if (versionNumberParts.Length == 2)
                {
                    migrationFile.BeginningMigrationVersion = MigrationVersion.FromVersion(versionNumberParts[1]);
                    migrationFile.EndingMigrationVersion = MigrationVersion.LATEST;
                }
                else
                {
                    migrationFile.BeginningMigrationVersion = MigrationVersion.FromVersion(versionNumberParts[1]);
                    migrationFile.BeginningMigrationVersion = MigrationVersion.FromVersion(versionNumberParts[2]);
                }
            }

            return migrationFile;
        }
    }
}
