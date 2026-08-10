using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    private const string GetGenresEndpointName = "GetGenre";

    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres").WithTags("Genres");

        group.MapGet("/", List).WithSummary("List Genres").WithDescription("List existing Genres.");

        group
            .MapGet("/{id:int}", Get)
            .WithName(GetGenresEndpointName)
            .WithSummary("Get Genre")
            .WithDescription("Find a Genre by Id.");

        group
            .MapPost("/", Create)
            .WithSummary("Create Genre")
            .WithDescription("Create a new Genre.");

        group
            .MapPut("/{id:int}", Update)
            .WithSummary("Update Genre")
            .WithDescription("Update an existing Genre.");

        group
            .MapDelete("/{id:int}", Delete)
            .WithSummary("Delete Genre")
            .WithDescription("Delete an existing Genre.");
    }

    public static async Task<List<GenreDetailsDto>> List(GameStoreContext db) =>
        await db
            .Genres.Select(genre => new GenreDetailsDto(Id: genre.Id, Name: genre.Name))
            .AsNoTracking()
            .ToListAsync();

    public static async Task<IResult> Get(GameStoreContext db, int id) =>
        await db.Genres.FindAsync(id) is { } game
            ? Results.Ok(new GenreDetailsDto(Id: game.Id, Name: game.Name))
            : Results.NotFound();

    public static async Task<IResult> Create(GameStoreContext db, CreateGenreDto dto)
    {
        var genre = new Genre() { Name = dto.Name };

        await db.Genres.AddAsync(genre);
        await db.SaveChangesAsync();

        var details = new GenreDetailsDto(Id: genre.Id, Name: genre.Name);

        return Results.CreatedAtRoute(GetGenresEndpointName, new { id = genre.Id }, details);
    }

    public static async Task<IResult> Delete(GameStoreContext db, int id)
    {
        await db.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();

        return Results.NoContent();
    }

    public static async Task<IResult> Update(GameStoreContext db, int id, UpdateGenreDto dto)
    {
        var existing = await db.Genres.FindAsync(id);

        if (existing is null)
        {
            return Results.NotFound();
        }

        existing.Name = dto.Name;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
