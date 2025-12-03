using BIApi;

namespace BlImplementation;

internal class Bl : IBi
{
    public IAdmin Admin { get; } = new AdminImplemenation();

    public ICourier Courier => new CourierImplementation();
    public IOrder Order => new OrderImplementation();
}
