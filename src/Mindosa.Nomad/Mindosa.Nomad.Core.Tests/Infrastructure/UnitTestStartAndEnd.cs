using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindosa.Nomad.Core.Tests.Infrastructure
{
    public static class UnitTestStartAndEnd
    {
        //TODO: need to expand this method to allow the possibility of one instance with multiple databases
        public static void Start(string databaseName)
        {
            // make sure any previous instances are shut down
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c sqllocaldb stop \"" + databaseName + "testinstance\""
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            // delete any previous instance
            startInfo.Arguments = "/c sqllocaldb delete \"" + databaseName + "testinstance\"";
            process.Start();
            process.WaitForExit();

            // check to see if the database files exist, if so, then delete them
            if (File.Exists(databaseName + ".mdf"))
            {
                File.Delete(databaseName + ".mdf");
            }

            if (File.Exists(databaseName + "_log.ldf"))
            {
                File.Delete(databaseName + "_log.ldf");
            }

            // create a new localdb sql server instance
            startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c sqllocaldb create \"" + databaseName + "testinstance\" -s"
            };

            process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            CreateDatabase(databaseName);
        }

        public static void End(string databaseName)
        {
            // shut down the instance
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/c sqllocaldb stop \"" + databaseName + "testinstance\""
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            // delete the instance
            startInfo.Arguments = "/c sqllocaldb delete \"" + databaseName + "testinstance\"";
            process.Start();
            process.WaitForExit();

            if (File.Exists(databaseName + ".mdf"))
            {
                File.Delete(databaseName + ".mdf");
            }

            if (File.Exists(databaseName + "_log.ldf"))
            {
                File.Delete(databaseName + "_log.ldf");
            }
        }

        public static void TruncateData(string databaseName, string[] tableList)
        {
            SqlConnection db = new SqlConnection(
                "server=(localdb)\\" + databaseName + "testinstance;" +
                "Trusted_Connection=yes;" +
                "database=" + databaseName + "; " +
                "Integrated Security=true; " +
                "connection timeout=30");

            db.Open();

            foreach (var item in tableList)
            {
                SqlCommand myCommand = new SqlCommand(@"TRUNCATE TABLE " + databaseName + ".." + item, db);
                myCommand.ExecuteNonQuery();
            }
            db.Close();
        }
        private static void CreateDatabase(string databaseName)
        {
            string databaseDirectory = Directory.GetCurrentDirectory();

            SqlConnection db = new SqlConnection(
                "server=(localdb)\\" + databaseName + "testinstance;" +
                "Trusted_Connection=yes;" +
                "database=master; " +
                "Integrated Security=true; " +
                "connection timeout=30");

            db.Open();

            try
            {
                SqlCommand myCommand = new SqlCommand(@"CREATE DATABASE [" + databaseName + @"]
                  CONTAINMENT = NONE
                  ON  PRIMARY 
                  ( NAME = N'" + databaseName + @"', FILENAME = N'" + databaseDirectory + @"\" + databaseName +
                                                              @".mdf' , SIZE = 8096KB , FILEGROWTH = 1024KB )
                  LOG ON 
                  ( NAME = N'" + databaseName + @"_log', FILENAME = N'" + databaseDirectory + @"\" + databaseName +
                                                              @"_log.ldf' , SIZE = 8096KB , FILEGROWTH = 10%)
                  ", db);

                myCommand.ExecuteNonQuery();
                db.Close();
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                throw;
            }
        }
    }
}

