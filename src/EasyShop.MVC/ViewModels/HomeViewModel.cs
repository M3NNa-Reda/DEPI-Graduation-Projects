namespace EasyShop.MVC;

public class HomeViewModel
{
    public string Title { get; set; } = "EasyShop";

    public string Subtitle { get; set; } = string.Empty;

    public IReadOnlyList<StatCardViewModel> Cards { get; set; } = new List<StatCardViewModel>();
}
