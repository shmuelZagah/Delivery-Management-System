namespace Dal;
using DalApi;
using DO;

sealed public class DalList : IDal
{
    public ICourier Courier => new CourierImplementation();
    public IOrder Order => new OrderImplementation();
    public IDelivery Delivery => new DeliveryImplementation();
    public IConfig Config => new ConfigImplementation();
    public void ResetDB()
    {
        Delivery.DeleteAll();
        Order.DeleteAll();
        Courier.DeleteAll();
        Config.Reset();
    }
}
