using BO;
namespace BIApi;

public interface ICourier
{

    //Login to the system
    UserType Login(string username, string password);

    //Get a list of couriers with optional filters
    IEnumerable<BO.CourierInList> GetCouriers(int requesterId, bool? isActive, CourierField? courierField);

    //Get details of a specific courier by ID
    Courier GetCourierDetails(int requesterId, int courierId);

    //Update courier information
    void UpdateCourier(int requesterId, Courier courier);

    //Delete a courier by ID
    void DeleteCourier(int requesterId, int courierId);

    //Add a new courier to the system
    void AddCourier(int requesterId, Courier courier);
}
