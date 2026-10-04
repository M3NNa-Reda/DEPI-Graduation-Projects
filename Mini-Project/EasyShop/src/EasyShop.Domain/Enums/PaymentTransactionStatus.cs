using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Enums
{
    public enum PaymentTransactionStatus
    {
        Pending,
        Succeeded,
        Failed,
        Refunded
    }
}
