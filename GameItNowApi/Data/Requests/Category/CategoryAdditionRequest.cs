using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Requests.Category;

public class CategoryAdditionRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Description { get; set; }
}