using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    private const string GenresGetEndpointName = "GetGenre";

    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        group.MapGet("/", List);

        group.MapGet("/{id:int}", Get).WithName(GenresGetEndpointName);

        group.MapPost("/", Create);

        group.MapPut("/{id:int}", Update);

        group.MapDelete("/{id:int}", Delete);
    }

    private static async Task<List<GenreDto>> List(GameStoreContext db) =>
        await db
            .Genres.Select(genre => new GenreDto(Id: genre.Id, Name: genre.Name))
            .AsNoTracking()
            .ToListAsync();

    private static async Task<IResult> Get(GameStoreContext db, int id) =>
        await db.Genres.FindAsync(id) is { } game ? Results.Ok(game) : Results.NotFound();

    private static async Task<IResult> Create(GameStoreContext db, CreateGenreDto dto)
    {
        var genre = new Genre() { Name = dto.Name };

        await db.Genres.AddAsync(genre);
        await db.SaveChangesAsync();

        return Results.CreatedAtRoute(GenresGetEndpointName, new { id = genre.Id }, genre);
    }

    private static async Task<IResult> Delete(GameStoreContext db, int id)
    {
        await db.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> Update(GameStoreContext db, int id, UpdateGenreDto dto)
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
