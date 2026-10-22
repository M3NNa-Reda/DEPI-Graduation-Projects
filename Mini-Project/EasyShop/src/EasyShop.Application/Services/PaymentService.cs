namespace EasyShop.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Payment> CreatePaymentAsync(int orderId, PaymentMethod method, decimal amount)
        {
            var existing = await _unitOfWork.Payments.GetByOrderIdAsync(orderId);
            if (existing is not null)
                throw new InvalidOperationException("A payment already exists for this order.");

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = method,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            return payment;
        }
        public Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return _unitOfWork.Payments.GetByOrderIdAsync(orderId);
        }
        public Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return _unitOfWork.Payments.GetByIdAsync(paymentId);
        }
        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return payments.OrderByDescending(p => p.CreatedAt).ToList();
        }
        public async Task<IEnumerable<PaymentTransaction>> GetTransactionsAsync(int paymentId)
        {
            var transactions = await _unitOfWork.PaymentTransactions.GetByPaymentIdAsync(paymentId);
            return transactions.OrderByDescending(t => t.CreatedAt).ToList();
        }
        public async Task MarkAsPaidAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Paid)
                throw new InvalidOperationException("This payment has already been marked as paid.");

            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;
            _unitOfWork.Payments.Update(payment);

            await _unitOfWork.PaymentTransactions.AddAsync(new PaymentTransaction
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Status = PaymentTransactionStatus.Succeeded,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CompleteAsync();
        }
        public async Task MarkAsFailedAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Cannot fail a payment that has already been paid.");

            payment.Status = PaymentStatus.Failed;
            _unitOfWork.Payments.Update(payment);

            await _unitOfWork.PaymentTransactions.AddAsync(new PaymentTransaction
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Status = PaymentTransactionStatus.Failed,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.CompleteAsync();
        }
    }
}