using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Shipment
{
    public class CreateShipmentDto
    {
        public int OrderId { get; set; }
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
