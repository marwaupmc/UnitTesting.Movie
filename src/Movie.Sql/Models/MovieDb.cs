// <copyright file="MovieDb.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Movie.Sql
{
    public class MovieDb
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ReleaseDate { get; set; }

        public Movie ToModel()
        {
            return new Movie(
                id: this.Id,
                title: this.Title,
                releaseDate: this.ReleaseDate);
        }
    }
}
