using BO;
using System;
namespace BlApi;

public interface ICourier : BlApi.IObservable
{

    //Login to the system
    Task<UserType> Login(string username, string password);

    //Get a list of couriers with optional filters
    IEnumerable<BO.CourierInList> GetCouriers(int requesterId, bool? isActive, ShipmentType? shipmentType);

    //Get details of a specific courier by ID
    Task<Courier> GetCourierDetails(int requesterId, int courierId);

    //Update courier information
    Task UpdateCourier(int requesterId, Courier courier);

    //Delete a courier by ID
    Task DeleteCourier(int requesterId, int courierId);

    //Add a new courier to the system
    Task AddCourier(int requesterId, Courier courier);
}
