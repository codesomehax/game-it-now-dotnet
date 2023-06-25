using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Requests.Game;

public class GameAdditionRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public List<string> Categories { get; set; }
}