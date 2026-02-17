using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

//--------------------------
// CourierImplementation
//--------------------------
internal class CourierImplementation : BlApi.ICourier
{

    // Login method for courier authentication
    public async Task<UserType> Login(string username, string password)
    {

        //Input validation
        Helpers.Tools.IdValidtion(int.TryParse(username, out int result)? result : 0);


        //Manager login
        if (Helpers.CourierManager.EnsureIsManager(username))
            if (password != Helpers.AdminManager.GetConfig().ManagerPassword)
                throw new BlInvalidInputException("Incorrect password");
            else return BO.UserType.Manager;



        //Courier login
        var courier = await Helpers.CourierManager.GetCourier(int.TryParse(username, out var idTemp) ? idTemp
                : throw new BlInvalidInputException("Invalid input, Username input is invalid"));

        if (password != courier!.Password)
            throw new BlInvalidInputException("Incorrect password");

        return BO.UserType.Courier;
    }


    // Retrieve a list of couriers
    public IEnumerable<CourierInList> GetCouriers(int requesterId, bool? isActive, ShipmentType? shipmentType)
    {

        // Authorization check: only managers can access this method
        if (!Helpers.CourierManager.EnsureIsManager(requesterId))
            throw new BlInvalidInputException("Unauthorized access attempt detected");

        // Get all couriers (if isActive is null get all)
        IEnumerable <CourierInList> couriers;

        if (isActive != null)
            couriers = Helpers.CourierManager.GetAllCouriers(p => p.IsActive == isActive);
        else
            couriers = Helpers.CourierManager.GetAllCouriers();

        if (shipmentType != null)
        {
            // Apply additional filtering or selection based on the shipmentType
            couriers = couriers.Where(c => c.ShipmentType == shipmentType);
        }

        return couriers;
    }


    public async Task AddCourier(int requesterId, Courier courier)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        // Authorization check: only managers can access this method
        Helpers.CourierManager.EnsureIsManager(requesterId);

        // Validate courier details
        Helpers.CourierManager.ValidationOfCarrier(courier);

        //If all validations passed, add the courier to the system
        Helpers.CourierManager.AddCourier(courier);
    }

    public async Task DeleteCourier(int requesterId, int courierId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        // Authorization check: only managers can access this method
        if (!Helpers.CourierManager.EnsureIsManager(requesterId.ToString()))
        {
            throw new BLUnauthorizedAccessException("Unauthorized access attempt detected");
        }

        // Delete the courier from the system
        await Helpers.CourierManager.DeleteCourier(courierId);

    }

    public async Task <Courier> GetCourierDetails(int requesterId, int courierId)
    {
        var courier = await Helpers.CourierManager.GetCourier(courierId);

        if (requesterId == courier!.Id || Helpers.CourierManager.EnsureIsManager(requesterId))
        {
            return courier;
        }
        else throw new BLUnauthorizedAccessException("Unauthorized access attempt detected");
    }

    public async Task UpdateCourier(int requesterId, Courier courier)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        var courierToUpdate = await Helpers.CourierManager.GetCourier(courier.Id);

        // Authorization check: only managers can change isActive status
        if (courierToUpdate!.IsActive != courier.IsActive)
            try
            {
                Helpers.CourierManager.EnsureIsManager(requesterId.ToString());
            }
            catch (BLUnauthorizedAccessException ex)
            {
                throw new BLUnauthorizedAccessException("Cannot change the activeness status", ex);
            }

        // Update courier details
        Helpers.CourierManager.UpdateCourier(courier);

    }

    public void AddObserver(Action listObserver) =>
        CourierManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        CourierManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        CourierManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        CourierManager.Observers.RemoveObserver(id, observer);

}