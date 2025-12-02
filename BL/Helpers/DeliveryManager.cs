using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

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


    /// <summary>
    /// Returns the delivery associated with a given OrderId.
    /// Throws if no delivery exists.
    /// </summary>
    internal static DO.Delivery GetDeliveryByOrderId(int orderId)
    {
        // Get DO delivery
        DO.Delivery? doDelivery = s_dal.Delivery.ReadAll()
            .FirstOrDefault(d => d.OrderId == orderId);

        if (doDelivery is null)
            throw new BO.BlDoesNotExistException($"No delivery found for Order ID={orderId}");

        return doDelivery;
      
    }

    /// <summary>
    /// Updates the delivery associated with the given OrderId.
    /// Only updates existing delivery; throws if no delivery exists.
    /// </summary>
    internal static void UpdateDelivery( DO.Delivery updatedDelivery)
    {
       s_dal.Delivery.Update(updatedDelivery);
    }

    /// <summary>
    /// delete order by orderId
    /// </summary>
    internal static void createDelivery(DO.Delivery updatedDelivery)
    {
        s_dal.Delivery.Create(updatedDelivery);
    }


}


