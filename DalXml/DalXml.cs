namespace Dal;
using DalApi;
using DO;

sealed internal class DalXml : IDal
{
    //create singleton class
    public static IDal? instance = null;

    //create lock object for thread safety
    private static readonly object lockObj = new object();
    public static IDal Instance
    {
        get
        {
            if (instance == null)       // Lazy initialization
            {
                lock (lockObj)         // Thread safe
                {
                    if (instance == null)
                        instance = new DalXml();
                }
            }
            return instance;
        }
    }
    private DalXml() { }

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
