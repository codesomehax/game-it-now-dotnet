using GameItNowApi.Model.JoinTables;
using Microsoft.Build.Framework;

namespace GameItNowApi.Model;

public class Game
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public string ImageUrl { get; set; }

    public ICollection<GameCategory> Categories { get; set; }
}