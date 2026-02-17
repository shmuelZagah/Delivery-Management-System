using DalApi;

namespace DalApi;

public interface IDal
{
    ICourier Courier { get; }
    IOrder Order { get; }
    IDelivery Delivery { get; }
    IConfig Config { get; }
    IAddress Address { get; }
    void ResetDB();
}
