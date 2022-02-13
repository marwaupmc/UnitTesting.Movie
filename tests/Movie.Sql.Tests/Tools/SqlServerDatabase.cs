// <copyright file="SqlServerDatabase.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Pock.Tools
{
    using System;
    using System.Data;
    using System.Globalization;
    using Microsoft.Data.SqlClient;

    internal sealed class SqlServerDatabase : IDisposable
    {
        private SqlConnection connection;
        private string connectionString;

        internal SqlServerDatabase(string connectionString)
        {
            this.connectionString = connectionString;

            this.connection = new SqlConnection(connectionString);
            this.connection.Open();
        }

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }

        public void ExecuteNonQuery(string sqlCommand, params object[] arguments)
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = string.Format(CultureInfo.InvariantCulture, sqlCommand, arguments);
            command.ExecuteNonQuery();
        }

        public DataTable ExecuteQuery(string query)
        {
            using var adapter = new SqlDataAdapter(query, this.connection);
            var dataTable = new DataTable();

            adapter.Fill(dataTable);

            return dataTable;
        }
    }
}
