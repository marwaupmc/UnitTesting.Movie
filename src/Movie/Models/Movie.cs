// <copyright file="Movie.cs" company="MarwaCHEBIL">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Movie
{
    public class Movie
    {
        public Movie(Guid id, string title, DateTime? releaseDate)
        {
            this.Id = id;
            this.Title = title;
            this.ReleaseDate = releaseDate;
        }

        public Guid Id { get; }

        public string Title { get; }

        public DateTime? ReleaseDate { get; }
    }
}
