using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record CreateGameDto(
    [Required] [StringLength(50)] string Name,
    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")] int GenreId,
    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")] decimal Price,
    DateOnly ReleaseDate
);
