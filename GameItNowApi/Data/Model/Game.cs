using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Model;

public class Game
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public string ImageUrl { get; set; }

    public ICollection<Category> Categories { get; set; }
    public ICollection<AppUser> AppUsers { get; set; }
}