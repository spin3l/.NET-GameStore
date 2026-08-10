using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    private const string GetGameEndpointName = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        group.MapGet("/", List);

        group.MapGet("/{id:int}", Get).WithName(GetGameEndpointName);

        group.MapPost("", Create);

        group.MapPut("/{id:int}", Update);

        group.MapDelete("/{id:int}", Delete);
    }

    public static async Task<IResult> Update(GameStoreContext db, int id, UpdateGameDto dto)
    {
        var existingGame = await db.Games.FindAsync(id);

        if (existingGame is null)
        {
            return Results.NotFound();
        }

        // Check genre existence
        if (!await db.Genres.AnyAsync(g => g.Id == dto.GenreId))
        {
            return Results.BadRequest($"Genre with id {dto.GenreId} does not exist.");
        }

        db.Entry(existingGame).CurrentValues.SetValues(dto);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    public static async Task<IResult> Delete(GameStoreContext db, int id)
    {
        await db.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

        return Results.NoContent();
    }

    public static async Task<IResult> Create(GameStoreContext db, CreateGameDto dto)
    {
        Game game = new()
        {
            Name = dto.Name,
            GenreId = dto.GenreId,
            Price = dto.Price,
            ReleaseDate = dto.ReleaseDate,
        };

        // Check genre existence
        if (!await db.Genres.AnyAsync(g => g.Id == game.GenreId))
        {
            return Results.BadRequest($"Genre with id {game.GenreId} does not exist.");
        }

        await db.AddAsync(game);
        await db.SaveChangesAsync();

        GameDetailsDto details = new(
            Id: game.Id,
            Name: game.Name,
            GenreId: game.GenreId,
            Price: game.Price,
            ReleaseDate: game.ReleaseDate
        );

        return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, details);
    }

    public static async Task<IResult> Get(GameStoreContext db, int id)
    {
        return await db.Games.FindAsync(id) is { } game
            ? Results.Ok(
                new GameDetailsDto(
                    Id: game.Id,
                    Name: game.Name,
                    GenreId: game.GenreId,
                    Price: game.Price,
                    ReleaseDate: game.ReleaseDate
                )
            )
            : Results.NotFound();
    }

    public static async Task<List<GameSummaryDto>> List(GameStoreContext db)
    {
        return await db
            .Games.Include(game => game.Genre)
            .Select(game => new GameSummaryDto(
                Id: game.Id,
                Name: game.Name,
                Genre: game.Genre!.Name,
                Price: game.Price,
                ReleaseDate: game.ReleaseDate
            ))
            .ToListAsync();
    }
}
