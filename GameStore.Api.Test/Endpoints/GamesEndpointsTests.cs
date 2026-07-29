using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;
using GameStore.Api.Test.Fixtures;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Test.Endpoints;

public class GamesEndpointsTests : GameStoreTestBase
{
    [Test]
    public async Task List_ReturnsAllGames_WithGenreName()
    {
        var (genre, game) = await GameStoreSeeding.NewGame(Db);

        var result = await GamesEndpoints.List(Db);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo(game.Name));
            Assert.That(result[0].Genre, Is.EqualTo(genre.Name));
        }
    }

    [Test]
    public async Task Get_ExistingId_ReturnsOkWithGame()
    {
        var (_, game) = await GameStoreSeeding.NewGame(Db);

        var result = await GamesEndpoints.Get(Db, game.Id);
        var okResult = result as Ok<GameDetailsDto>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value!.Name, Is.EqualTo(game.Name));
        }
    }

    [Test]
    public async Task Get_NonExistingId_ReturnsNotFound()
    {
        var result = await GamesEndpoints.Get(Db, 999);

        Assert.That(result, Is.InstanceOf<NotFound>());
    }

    [Test]
    public async Task Create_AddsGame_ReturnsCreatedAtRoute()
    {
        var genre = await GameStoreSeeding.SeedGenreAsync(Db, "RPG");
        var dto = new CreateGameDto("Elden Ring", genre.Id, 69.99m, new DateOnly(2022, 2, 25));

        var result = await GamesEndpoints.Create(Db, dto);

        var created = result as CreatedAtRoute<GameDetailsDto>;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Value!.Name, Is.EqualTo(dto.Name));
            Assert.That(await Db.Games.CountAsync(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task Update_ExistingGame_UpdatesFields()
    {
        var (genre, game) = await GameStoreSeeding.NewGame(Db);

        var dto = new UpdateGameDto("New Name", genre.Id, 20m, new DateOnly(2021, 1, 1));
        var result = await GamesEndpoints.Update(Db, game.Id, dto);

        var updated = await Db.Games.FindAsync(game.Id);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<NoContent>());
            Assert.That(updated!.Name, Is.EqualTo("New Name"));
            Assert.That(updated.Price, Is.EqualTo(20m));
        }
    }

    [Test]
    public async Task Update_NonExistingGame_ReturnsNotFound()
    {
        var dto = new UpdateGameDto("X", 1, 1m, new DateOnly(2020, 1, 1));

        var result = await GamesEndpoints.Update(Db, 999, dto);

        Assert.That(result, Is.InstanceOf<NotFound>());
    }

    [Test]
    public async Task Delete_RemovesGame()
    {
        var (_, game) = await GameStoreSeeding.NewGame(Db);

        var result = await GamesEndpoints.Delete(Db, game.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.InstanceOf<NoContent>());
            Assert.That(
                await Db.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == game.Id),
                Is.Null
            );
        }
    }
}
