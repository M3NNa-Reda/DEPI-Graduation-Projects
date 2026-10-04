namespace EasyShop.Domain.Entities
{
    public enum PaymentTransactionStatus
    {
        Pending,
        Succeeded,
        Failed,
        Refunded
    }
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string? TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public string? GatewayResponse { get; set; }
        public DateTime CreatedAt { get; set; }

        public Payment Payment { get; set; }
    }
}
