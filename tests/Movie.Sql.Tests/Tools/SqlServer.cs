// <copyright file="SqlServer.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Pock.Tools
{
    using System;
    using Microsoft.Data.SqlClient;
    using Microsoft.SqlServer.Dac;

    internal static class SqlServer
    {
        private const string DatabaseName = "MovieUnitTests";

        public static void DeployDacPackage(string fileName)
        {
            using (var dacPackage = DacPackage.Load(fileName))
            {
                var dacDeployOptions = new DacDeployOptions()
                {
                    CreateNewDatabase = true,
                };

                var dacServices = new DacServices(GetDefaultDatabaseConnectionString(true));
                dacServices.Deploy(dacPackage, DatabaseName, true, dacDeployOptions);
            }
        }

        public static SqlServerDatabase Connect()
        {
            return new SqlServerDatabase(GetDefaultDatabaseConnectionString(false));
        }

        public static string GetDefaultDatabaseConnectionString()
        {
            return GetDefaultDatabaseConnectionString(false);
        }

        private static string GetDefaultDatabaseConnectionString(bool isAdmin)
        {
            // Retrieves the connection string from the "UNITTESTS_SQLSERVER_DATASOURCE".
            // This environment variable is specified in the build process,
            // because we can change the targeted database for each build process.
            var dataSource = Environment.GetEnvironmentVariable("UNITTESTS_SQLSERVER_DATASOURCE");

            if (string.IsNullOrWhiteSpace(dataSource))
            {
                // If not specified (we are on the computer developers), use the "(localdb)\v11.0" by default
                dataSource = "(localdb)\\cst-unit-tests";
            }

            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = dataSource,
                InitialCatalog = DatabaseName,
                IntegratedSecurity = true,
            };

            return connectionString.ToString();
        }
    }
}
