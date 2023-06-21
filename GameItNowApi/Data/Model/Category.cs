using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Model;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ICollection<Game> Games { get; set; }
}