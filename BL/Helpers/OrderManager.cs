using DalApi;

namespace Helpers
{
    internal class OrderManager
    {
        private static IDal s_dal = Factory.Get;

        internal static BO.Order? GetOrder(int id)
        {
            DO.Order doOrder;

            try
            {
                doOrder = s_dal.Order.Read(id)
                    ?? throw new BO.BlDoesNotExistException($"Order with ID={id} does not exist");
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Order with ID={id} does not exist", ex);
            }

            return new BO.Order()
            {
                Id = doOrder.Id,
                OrderType = (BO.OrderType)doOrder.OrderType,
                Description = doOrder.Description,
                Address = doOrder.FullAddress,
                Latitude = doOrder.Latitude,
                Longitude = doOrder.Longitude,
                CustomerName = doOrder.CustomerName,
                CustomerPhone = doOrder.CustomerPhone,
                Notes = doOrder.Notes,
                OrderCreated = doOrder.OrderCreationTime
            };
        }
    }
}
