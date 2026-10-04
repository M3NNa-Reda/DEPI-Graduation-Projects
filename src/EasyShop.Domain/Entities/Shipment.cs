using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace EasyShop.Domain.Entities
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public string ShippingProvider { get; set; }
        public decimal ShippingFee { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public Order Order { get; set; }
    }
}
