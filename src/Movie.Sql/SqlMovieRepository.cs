// <copyright file="SqlMovieRepository.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Movie.Sql
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class SqlMovieRepository : IMovieRepository
    {
        private readonly IOptions<SqlMovieRepositoryOptions> options;

        public SqlMovieRepository(IOptions<SqlMovieRepositoryOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Value.Validate();

            this.options = options;
        }

        public async Task<Movie> GetMovieByIdAsync(Guid id)
        {
            using (var context = new MovieContext(this.options))
            {
                var movieDb = context.Movie.Where(x => x.Id == id);

                if (!await movieDb.AnyAsync())
                {
                    throw new InvalidOperationException($"There is no Movie with Id = '{id}'");
                }

                return (await movieDb.SingleAsync()).ToModel();
            }
        }

        public async Task SaveMovieAsync(Movie movie)
        {
            using (var context = new MovieContext(this.options))
            {
                var movieDb = context.Movie.Where(x => x.Id == movie.Id);
                if (!await movieDb.AnyAsync())
                {
                    var newEntry = new MovieDb()
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        ReleaseDate = movie.ReleaseDate,
                    };

                    await context.Movie.AddAsync(newEntry);
                }
                else
                {
                    var mappedMovieDb = new MovieDb()
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        ReleaseDate = movie.ReleaseDate,
                    };

                    context.Entry(await movieDb.SingleAsync()).CurrentValues.SetValues(mappedMovieDb);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteMovieAsync(Movie movie)
        {
            using (var context = new MovieContext(this.options))
            {
                var movieDb = context.Movie.Where(x => x.Id == movie.Id);
                if (await movieDb.AnyAsync())
                {
                    context.Movie.Remove(await movieDb.SingleAsync());
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
