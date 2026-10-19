using EasyShop.Application.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IShipmentService
    {
        Task<IEnumerable<ShipmentDto>> GetAllShipmentsAsync();
        Task<ShipmentDto?> GetShipmentByIdAsync(int id);
        Task<ShipmentDto> CreateShipmentAsync(CreateShipmentDto dto);
        Task<bool> UpdateShipmentAsync(int id, UpdateShipmentDto dto);
        Task<bool> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusDto dto);
        Task<bool> DeleteShipmentAsync(int id);
            
    }
}
