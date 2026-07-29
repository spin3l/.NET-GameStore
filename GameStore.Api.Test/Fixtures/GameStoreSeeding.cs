using GameStore.Api.Data;
using GameStore.Api.Models;

namespace GameStore.Api.Test.Fixtures;

public class GameStoreSeeding
{
    public static async Task<Genre> SeedGenreAsync(GameStoreContext db, string name = "Action")
    {
        var genre = new Genre { Name = name };
        db.Genres.Add(genre);
        await db.SaveChangesAsync();
        return genre;
    }

    public static async Task<Game> SeedGameAsync(
        GameStoreContext db,
        string name,
        int genreId,
        decimal price,
        DateOnly releaseDate
    )
    {
        var game = new Game()
        {
            Name = name,
            GenreId = genreId,
            Price = price,
            ReleaseDate = releaseDate,
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();
        return game;
    }

    public static async Task<(Genre, Game)> NewGame(GameStoreContext db)
    {
        var genre = await SeedGenreAsync(db, "RPG");
        var game = await SeedGameAsync(
            db,
            "Baldur's Gate 3",
            genre.Id,
            49.99m,
            new DateOnly(2023, 8, 3)
        );
        return (genre, game);
    }
}
