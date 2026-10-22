namespace EasyShop.MVC.ViewModels;

public class PaymentDetailsViewModel
{
    public Payment Payment { get; init; } = null!;

    public IReadOnlyList<PaymentTransaction> Transactions { get; init; } = Array.Empty<PaymentTransaction>();
}
