using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal class DeliveryManager
    {
        private static IDal s_dal = Factory.Get;

        internal static DO.Delivery? GetDelivery(int id)
        {
            DO.Delivery doDelivery;

            try
            {
                doDelivery = s_dal.Delivery.Read(id)
                    ?? throw new BO.BlDoesNotExistException($"Delivery with ID={id} does not exist");
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Delivery with ID={id} does not exist", ex);
            }

            return new DO.Delivery()
            {
                Id = doDelivery.Id,
                OrderId = doDelivery.OrderId,
                CourierId = doDelivery.CourierId,

                DeliveryType = (DO.ShipmentType)doDelivery.DeliveryType,

                StartTime = doDelivery.StartTime,
                DistanceKm = doDelivery.DistanceKm,

                DeliveryEndType = (DO.DeliveryEndType?)doDelivery.DeliveryEndType,
                EndTime = doDelivery.EndTime
            };
        }

    }
}
