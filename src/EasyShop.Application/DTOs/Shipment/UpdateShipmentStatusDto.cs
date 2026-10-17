using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Shipment
{
    public class UpdateShipmentStatusDto
    {
        public ShipmentStatus Status { get; set; }
    }
}
