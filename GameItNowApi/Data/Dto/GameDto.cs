namespace GameItNowApi.Data.Dto;

public class GameDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }

    public ICollection<string> Categories { get; set; }
}