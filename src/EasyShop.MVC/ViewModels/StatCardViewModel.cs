namespace EasyShop.MVC;

public class StatCardViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Count { get; set; } = "0";

    public string? Description { get; set; }

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";
}
