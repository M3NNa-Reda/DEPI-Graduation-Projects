using EasyShop.Application.DTOs.Shipment;

namespace EasyShop.Application.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ShipmentDto>> GetAllShipmentsAsync()
        {
            var shipments = await _unitOfWork.Shipments.GetAllAsync();

            return shipments.Select
                (s => new ShipmentDto
                {
                    Id = s.Id,
                    OrderId = s.OrderId,
                    Carrier = s.ShippingProvider,
                    TrackingNumber = s.TrackingNumber,
                    Status = s.Status,
                    ShippingCost = s.ShippingFee,
                    ShippedAt = s.ShippedAt,
                    DeliveredAt = s.DeliveredAt
                }
                );
        }

        public async Task<ShipmentDto?> GetShipmentByIdAsync(int id)
        {
            var shipment = await _unitOfWork.Shipments.GetByIdAsync(id);

            if (shipment == null)
                return null;

            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                Carrier = shipment.ShippingProvider,
                TrackingNumber = shipment.TrackingNumber,
                Status = shipment.Status,
                ShippingCost = shipment.ShippingFee,
                ShippedAt = shipment.ShippedAt,
                DeliveredAt = shipment.DeliveredAt
            };
        }
        public async Task<ShipmentDto> CreateShipmentAsync(CreateShipmentDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);

            if(order == null)
                throw new ArgumentException("Order Can't Found..");
            
            if (string.IsNullOrWhiteSpace(dto.Carrier))
                throw new ArgumentException("Carrier is Required ");
            
            if (dto.ShippingCost < 0)
                throw new ArgumentException("Shipping Cost Can't be Negative");

            var shipment = new Shipment
            {
                OrderId = dto.OrderId,
                ShippingProvider = dto.Carrier,
                TrackingNumber = dto.TrackingNumber,
                ShippingFee = dto.ShippingCost,
                Status = ShipmentStatus.Pending
            };

            await _unitOfWork.Shipments.AddAsync(shipment);
          
            await _unitOfWork.CompleteAsync();

            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                Carrier = shipment.ShippingProvider,
                TrackingNumber = shipment.TrackingNumber,
                Status = shipment.Status,
                ShippingCost = shipment.ShippingFee,
                ShippedAt = shipment.ShippedAt,
                DeliveredAt = shipment.DeliveredAt
            };
        }

        public async Task<bool> UpdateShipmentAsync(int id, UpdateShipmentDto dto)
        {
            var shipment = await _unitOfWork.Shipments.GetByIdAsync(id);

            if (shipment == null)
                return false;
           
            if (dto.Carrier != null)
            {
                if(string.IsNullOrWhiteSpace(dto.Carrier))
                    throw new ArgumentException("Carrier cannot be empty.");

                shipment.ShippingProvider = dto.Carrier;
            }

            if (dto.TrackingNumber != null)
                shipment.TrackingNumber = dto.TrackingNumber;

            if (dto.ShippingCost.HasValue) 
            {
                if(dto.ShippingCost.Value < 0)
                    throw new ArgumentException("Shipping cost cannot be negative.");

                shipment.ShippingFee = dto.ShippingCost.Value;
            }

            _unitOfWork.Shipments.Update(shipment);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusDto dto)
        {
            var shipment = await _unitOfWork.Shipments.GetByIdAsync(id);

            if (shipment == null)
                return false;
            
            if(!Enum.IsDefined(typeof(ShipmentStatus),dto.Status))
                throw new ArgumentException("Invalid shipment status.");

            shipment.Status = dto.Status;

            if (dto.Status == ShipmentStatus.Shipped && shipment.ShippedAt == null)
                shipment.ShippedAt = DateTime.UtcNow;

            if(dto.Status == ShipmentStatus.Delivered)
            {
                shipment.DeliveredAt = DateTime.UtcNow;

                if (shipment.ShippedAt == null)
                    shipment.ShippedAt = shipment.DeliveredAt;
            }

            _unitOfWork.Shipments.Update(shipment);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        public async Task<bool> DeleteShipmentAsync(int id)
        {
            var shipment = await _unitOfWork.Shipments.GetByIdAsync(id);

            if (shipment == null)
                return false;

            _unitOfWork.Shipments.Delete(shipment);
            await _unitOfWork.CompleteAsync();

            return true;
        }

      
    }
}
