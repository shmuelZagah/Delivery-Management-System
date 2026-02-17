using BlApi;
using Helpers;

namespace BlImplementation;

internal class Bl : IBl
{
    public IAdmin Admin { get; } = new AdminImplemenation();

    public ICourier Courier => new CourierImplementation();
    public IOrder Order => new OrderImplementation();

    public Bl()
    {
        AdminManager.ResetAddresses();
    }
}
