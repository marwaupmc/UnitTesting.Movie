// <copyright file="MovieContext.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Movie.Sql
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    internal class MovieContext : DbContext
    {
        private readonly IOptions<SqlMovieRepositoryOptions> options;

        public MovieContext(IOptions<SqlMovieRepositoryOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Value.Validate();
            this.options = options;
        }

        internal DbSet<MovieDb> Movie { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<MovieDb>().HasKey(m => m.Id);
            modelBuilder.Entity<MovieDb>().Property(m => m.Title).IsRequired();
            modelBuilder.Entity<MovieDb>().Property(m => m.ReleaseDate).IsRequired(false);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.options.Value.ConnectionString);
        }
    }
}
