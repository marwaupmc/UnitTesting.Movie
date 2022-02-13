// <copyright file="SqlMovieRepositoryOptions.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Movie.Sql
{
    public class SqlMovieRepositoryOptions
    {
        public string? ConnectionString { get; set; }

        internal void Validate()
        {
            if (this.ConnectionString is null)
            {
                throw new InvalidOperationException($"Instance of {nameof(SqlMovieRepositoryOptions)} is invalid, {nameof(SqlMovieRepositoryOptions.ConnectionString)} is null");
            }
        }
    }
}
