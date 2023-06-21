namespace GameItNowApi.Requests.Game;

public class GamePatchRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Categories { get; set; }
}