// <copyright file="IMovieRepository.cs" company="MarwaCHEBIL">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Movie
{
    public interface IMovieRepository
    {
        Task<Movie> GetMovieByIdAsync(Guid id);

        Task SaveMovieAsync(Movie movie);

        Task DeleteMovieAsync(Movie movie);
    }
}
