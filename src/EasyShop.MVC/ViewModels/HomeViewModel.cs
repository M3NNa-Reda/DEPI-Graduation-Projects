namespace EasyShop.MVC;

public class HomeViewModel
{
    public IReadOnlyList<StatCardViewModel> Cards { get; set; } = new List<StatCardViewModel>();
}
