
namespace BIApi;

public interface IBi
{
    IAdmin Admin { get; }
    ICourier Courier { get; }

    IOrder Order { get; }
}
