using System;
using System.Collections.Generic;
using System.Text;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;

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
        public async Task MarkAsPaidAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.CompleteAsync();
        }
        public async Task MarkAsFailedAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
                ?? throw new InvalidOperationException("Payment not found.");

            payment.Status = PaymentStatus.Failed;

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.CompleteAsync();
        }
    }
}