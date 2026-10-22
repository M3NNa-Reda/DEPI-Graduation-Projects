using EasyShop.MVC.ViewModels;

namespace EasyShop.MVC.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: /Payments?status=Pending
    [HttpGet]
    public async Task<IActionResult> Index(PaymentStatus? status)
    {
        var payments = await _paymentService.GetAllPaymentsAsync();

        if (status.HasValue)
        {
            payments = payments.Where(p => p.Status == status.Value).ToList();
        }

        ViewBag.SelectedStatus = status;
        return View(payments);
    }

    // GET: /Payments/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        var transactions = await _paymentService.GetTransactionsAsync(id);

        return View(new PaymentDetailsViewModel
        {
            Payment = payment,
            Transactions = transactions.ToList()
        });
    }

    // POST: /Payments/MarkAsPaid/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        return await ChangeStatusAsync(id, _paymentService.MarkAsPaidAsync, "Payment marked as paid.");
    }

    // POST: /Payments/MarkAsFailed/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsFailed(int id)
    {
        return await ChangeStatusAsync(id, _paymentService.MarkAsFailedAsync, "Payment marked as failed.");
    }

    private async Task<IActionResult> ChangeStatusAsync(int id, Func<int, Task> change, string successMessage)
    {
        try
        {
            await change(id);
            TempData["Success"] = successMessage;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
