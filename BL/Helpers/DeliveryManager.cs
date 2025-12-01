using BO;
using DalApi;
using DO;
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

        internal static BO.DeliveryPerOrderInList? GetDelivery(int id)
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

            return new BO.DeliveryPerOrderInList()
            {

                DeliveryId = doDelivery.Id,
                CourierId = doDelivery.CourierId,
                CourierName = s_dal.Courier.Read(doDelivery.CourierId)?.Name ?? "",



            };
        }

    }
}
