using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Shipment
{
    public class UpdateShipmentDto
    {
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal? ShippingCost { get; set; }
    }
}
