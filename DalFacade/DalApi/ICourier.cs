namespace DalApi;
using DO;

public interface ICourier : ICrud<Courier>
{
    void Create(Order order);
}
