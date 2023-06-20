namespace GameItNowApi.Model.JoinTables;

public class GameCategory
{
    public int GameId { get; set; }
    public int CategoryId { get; set; }
    
    public Game Game { get; set; }
    public Category Category { get; set; }
}