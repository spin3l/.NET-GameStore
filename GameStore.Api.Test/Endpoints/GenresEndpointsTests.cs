using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;
using GameStore.Api.Test.Fixtures;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Test.Endpoints;

public class GenresEndpointsTests : GameStoreTestBase
{
    [Test]
    public async Task List_ReturnsAllGenres()
    {
        var t1 = GameStoreSeeding.SeedGenreAsync(Db);
        var t2 = GameStoreSeeding.SeedGenreAsync(Db, "Platformer");
        await Task.WhenAll(t1, t2);

        var genre1 = t1.Result;
        var genre2 = t2.Result;

        var result = await GenresEndpoints.List(Db);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo(genre1.Name));
            Assert.That(result[1].Name, Is.EqualTo(genre2.Name));
        }
    }

    [Test]
    public async Task Get_ExistingId_ReturnsOkWithGenre()
    {
        var genre = await GameStoreSeeding.SeedGenreAsync(Db, "Simulation");

        var result = await GenresEndpoints.Get(Db, genre.Id);
        var okResult = result as Ok<GenreDetailsDto>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value!.Name, Is.EqualTo(genre.Name));
        }
    }

    [Test]
    public async Task Get_NonExistingId_ReturnsNotFound()
    {
        var result = await GenresEndpoints.Get(Db, 999);

        Assert.That(result, Is.InstanceOf<NotFound>());
    }

    [Test]
    public async Task Create_AddsGenre_ReturnsCreatedAtRoute()
    {
        var dto = new CreateGenreDto("Elden Ring");

        var result = await GenresEndpoints.Create(Db, dto);

        var created = result as CreatedAtRoute<GenreDetailsDto>;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Value!.Name, Is.EqualTo(dto.Name));
            Assert.That(await Db.Genres.CountAsync(), Is.EqualTo(1));
        }
    }
}
