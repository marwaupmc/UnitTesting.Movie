// <copyright file="SqlMovieRepositoryTest.cs" company="MarwaCHEBIL">
// Copyright (c) MarwaCHEBIL. All rights reserved.
// </copyright>

namespace Movie.Sql.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Pock.Tools;
    using Xunit;

    [Collection("SerialExecutionPublishDb")]
    public class SqlMovieRepositoryTest
    {
        [Fact]
        public void Constructor()
        {
            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = "connectionString" });
            var repo = new SqlMovieRepository(options);

            repo.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_OptionsNullException()
        {
            IOptions<SqlMovieRepositoryOptions>? options = null;

            Action action = () => { new SqlMovieRepository(options!); };

            action.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'options')");
        }

        [Fact]
        public void Constructor_OptionsInvalid()
        {
            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = null });

            Action creationWithException = () => { new SqlMovieRepository(options); };

            creationWithException.Should().ThrowExactly<InvalidOperationException>().WithMessage("Instance of SqlMovieRepositoryOptions is invalid, ConnectionString is null");
        }

        [Fact]

        public void GetMovieByIdAsync_CaseThrowInvalidOperationException()
        {
            SqlServer.DeployDacPackage(@"Movie.Sql.Database.dacpac");
            using var database = SqlServer.Connect();

            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'11111111-1111-1111-1111-111111111111', 'TitleT', N'2022-01-29')");
            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'22222222-2222-2222-2222-222222222222', 'TitleM', N'2022-01-30')");

            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = SqlServer.GetDefaultDatabaseConnectionString() });
            var repo = new SqlMovieRepository(options);

            var movieId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            Action act = () => repo.GetMovieByIdAsync(movieId).GetAwaiter().GetResult();

            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("There is no Movie with Id = '33333333-3333-3333-3333-333333333333'");
        }

        [Fact]
        public async Task GetMovieByIdAsync()
        {
            SqlServer.DeployDacPackage(@"Movie.Sql.Database.dacpac");
            using var database = SqlServer.Connect();

            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'11111111-1111-1111-1111-111111111111', 'TitleT', N'2022-01-29')");
            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'22222222-2222-2222-2222-222222222222', 'TitleM', N'2022-01-30')");

            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = SqlServer.GetDefaultDatabaseConnectionString() });
            var repo = new SqlMovieRepository(options);

            var expectedResult = new Movie(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                title: "TitleT",
                releaseDate: DateTime.Parse("2022-01-29"));

            var movieId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var result = await repo.GetMovieByIdAsync(movieId);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task SaveMovieAsync_CaseAdd()
        {
            SqlServer.DeployDacPackage(@"Movie.Sql.Database.dacpac");
            using var database = SqlServer.Connect();

            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'a1111111-1111-1111-1111-111111111111', 'TitleT', N'2022-01-29')");

            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = SqlServer.GetDefaultDatabaseConnectionString() });
            var repo = new SqlMovieRepository(options);

            var movie = new Movie(
                id: Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                title: "TitleM",
                releaseDate: DateTime.Parse("2022-02-06"));

            await repo.SaveMovieAsync(movie);

            var datatableMovieAll = database.ExecuteQuery("SELECT * FROM [dbo].[Movie]");
            datatableMovieAll.Rows.Count.Should().Be(2);

            var datatableMovie = database.ExecuteQuery("SELECT * FROM [dbo].[Movie] where Id = N'b1111111-1111-1111-1111-111111111111'");
            datatableMovie.Rows.Count.Should().Be(1);
            var row0 = datatableMovie.Rows[0];
            row0["Title"].As<string>().Should().Be("TitleM");
            row0["ReleaseDate"].As<DateTime>().Should().Be(DateTime.Parse("2022-02-06"));
        }

        [Fact]
        public async Task SaveMovieAsync_CaseUpdate()
        {
            SqlServer.DeployDacPackage(@"Movie.Sql.Database.dacpac");
            using var database = SqlServer.Connect();

            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'a1111111-1111-1111-1111-111111111111', 'TitleT', N'2022-01-29')");

            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = SqlServer.GetDefaultDatabaseConnectionString() });
            var repo = new SqlMovieRepository(options);

            var movie = new Movie(
                id: Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                title: "TitleM",
                releaseDate: DateTime.Parse("2022-02-25"));

            await repo.SaveMovieAsync(movie);

            var datatableMovie = database.ExecuteQuery("SELECT * FROM [dbo].[Movie]");
            datatableMovie.Rows.Count.Should().Be(1);
            var row0 = datatableMovie.Rows[0];
            row0["Id"].As<Guid>().Should().Be(Guid.Parse("a1111111-1111-1111-1111-111111111111"));
            row0["Title"].As<string>().Should().Be("TitleM");
            row0["ReleaseDate"].As<DateTime>().Should().Be(DateTime.Parse("2022-02-25"));
        }

        [Fact]
        public async Task DeleteMovieAsync()
        {
            SqlServer.DeployDacPackage(@"Movie.Sql.Database.dacpac");
            using var database = SqlServer.Connect();

            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'a1111111-1111-1111-1111-111111111111', 'TitleT', N'2022-02-02')");
            database.ExecuteNonQuery("insert into [dbo].[Movie]([Id], [Title], [ReleaseDate]) VALUES (N'b1111111-1111-1111-1111-111111111111', 'TitleM', N'2022-02-05')");

            var options = Options.Create(new SqlMovieRepositoryOptions() { ConnectionString = SqlServer.GetDefaultDatabaseConnectionString() });
            var repo = new SqlMovieRepository(options);

            var movie = new Movie(
                id: Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                title: "TitleT",
                releaseDate: DateTime.Parse("2022-02-02"));

            await repo.DeleteMovieAsync(movie);

            var datatableMovie = database.ExecuteQuery("SELECT * FROM [dbo].[Movie]");
            datatableMovie.Rows.Count.Should().Be(1);
            var row0 = datatableMovie.Rows[0];
            row0["Id"].As<Guid>().Should().Be(Guid.Parse("b1111111-1111-1111-1111-111111111111"));
            row0["Title"].As<string>().Should().Be("TitleM");
            row0["ReleaseDate"].As<DateTime>().Should().Be(DateTime.Parse("2022-02-05"));
        }
    }
}
