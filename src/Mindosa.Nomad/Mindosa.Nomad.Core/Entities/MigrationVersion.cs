using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Entities
{
    public class MigrationVersion : IComparable<MigrationVersion>
    {
        /// <summary>
        /// Version for an empty schema.
        /// </summary>
        public static readonly MigrationVersion EMPTY = new MigrationVersion(null, "<< Empty Schema >>");

        /// <summary>
        /// Latest version.
        /// </summary>
        public static readonly MigrationVersion LATEST = new MigrationVersion(-1, "<< Latest Version >>");

        /// <summary>
        /// Current version. Only a marker. For the real version use Flyway.info().current() instead.
        /// </summary>
        public static readonly MigrationVersion CURRENT = new MigrationVersion(-2, "<< Current Version >>");

        /// <summary>
        /// Compiled pattern for matching proper version format
        /// </summary>
        private static Regex splitPattern = new Regex("\\.(?=\\d)");

        /// <summary>
        /// The individual parts this version string is composed of. Ex. 1.2.3.4.0 -> [1, 2, 3, 4, 0]
        /// </summary>
        private readonly List<long> versionParts;

        /// <summary>
        /// The printable text to represent the version.
        /// </summary>
        private readonly string displayText;

        /// <summary>
        /// Factory for creating a MigrationVersion from a version string
        /// </summary>
        /// <param name="version">The version string. The value {@code current} will be interpreted as MigrationVersion.CURRENT, a marker for the latest version that has been applied to the database.</param>
        /// <returns></returns>
        public static MigrationVersion FromVersion(string version)
        {
            if ("current".Equals(version, StringComparison.CurrentCultureIgnoreCase)) return CURRENT;
            if (LATEST.GetVersion().Equals(version)) return LATEST;
            if (version == null) return EMPTY;
            return new MigrationVersion(version);
        }

        /// <summary>
        /// Creates a Version using this version string.
        /// </summary>
        /// <param name="version">
        /// The version in one of the following formats: 6, 6.0, 005, 1.2.3.4, 201004200021. <br/>{@code null}
        /// means that this version refers to an empty schema.
        /// </param>
        private MigrationVersion(string version)
        {
            string normalizedVersion = version.Replace('_', '.');
            this.versionParts = Tokenize(normalizedVersion);
            this.displayText = string.Join(".", versionParts);
        }

        /// <summary>
        /// Creates a Version using this version string.
        /// </summary>
        /// <param name="version">
        /// The version in one of the following formats: 6, 6.0, 005, 1.2.3.4, 201004200021. <br/>{@code null}
        /// means that this version refers to an empty schema.
        /// </param>
        /// <param name="displayText">The alternative text to display instead of the version number.</param>
        private MigrationVersion(long? version, string displayText)
        {
            this.versionParts = new List<long>();
            if (version.HasValue)
            {
                this.versionParts.Add(version.Value);
            }
            this.displayText = displayText;
        }

        public int CompareTo(MigrationVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this == EMPTY)
            {
                return other == EMPTY ? 0 : int.MinValue;
            }

            if (this == CURRENT)
            {
                return other == CURRENT ? 0 : int.MinValue;
            }

            if (this == LATEST)
            {
                return other == LATEST ? 0 : int.MaxValue;
            }

            if (other == EMPTY)
            {
                return int.MaxValue;
            }

            if (other == CURRENT)
            {
                return int.MaxValue;
            }

            if (other == LATEST)
            {
                return int.MinValue;
            }
            List<long> elements1 = versionParts;
            List<long> elements2 = other.versionParts;
            int largestNumberOfElements = Math.Max(elements1.Count, elements2.Count);
            for (int i = 0; i < largestNumberOfElements; i++)
            {
                int compared = getOrZero(elements1, i).CompareTo(getOrZero(elements2, i));
                if (compared != 0)
                {
                    return compared;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            return displayText;
        }

        public string GetVersion()
        {
            if (this.Equals(EMPTY)) return null;
            if (this.Equals(LATEST)) return long.MaxValue.ToString();
            return displayText;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            var version1 = (MigrationVersion)obj;

            return CompareTo(version1) == 0;
        }

        public override int GetHashCode()
        {
            return versionParts == null ? 0 : versionParts.GetHashCode();
        }

        private long getOrZero(List<long> elements, int i)
        {
            return i < elements.Count ? elements[i] : 0;
        }

        private List<long> Tokenize(string str)
        {
            var numbers = new List<long>();
            long tmpLong;

            foreach (var num in splitPattern.Split(str))
            {
                if (!long.TryParse(num, out tmpLong))
                {
                    throw new VersionNotFoundException("Invalid version containing non-numeric characters. Only 0..9 and . are allowed. Invalid version: "
                                                       + str);
                }
                numbers.Add(tmpLong);
            }

            for (int i = numbers.Count - 1; i > 0; i--)
            {
                if (numbers[i] != 0)
                    break;
                numbers.RemoveAt(i);
            }
            return numbers;
        }
    }
}
