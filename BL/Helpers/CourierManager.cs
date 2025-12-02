using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;

    #region Getters
    internal static BO.Courier? GetCourier(int id)
    {
        DO.Courier? doCourier = null;

        doCourier = s_dal.Courier.Read(id) ??
            throw new BO.BlDoesNotExistException ($"Courier with ID={id} does Not exist");



        return new BO.Courier()
        {
            Id = id,
            Name = doCourier.Name,
            Phone = doCourier.Phone,
            Email = doCourier.Email,
            Password = doCourier.Password,
            IsActive = doCourier.IsActive,
            MaxDistance = doCourier.PersonalMaxDistance,
            ShipmentType = (BO.ShipmentType)doCourier.PreferredShipmentType,
            StartTime = doCourier.EmploymentStartTime,
        };

    }

    internal static IEnumerable<BO.Courier> GetAllCouriers(Func<BO.Courier, bool>? predicate = null)
    {
        var dalCouriers = s_dal.Courier.ReadAll();
        var boCouriers = from doCourier in dalCouriers
                         select new BO.Courier()
                         {
                             Id = doCourier.Id,
                             Name = doCourier.Name,
                             Phone = doCourier.Phone,
                             Email = doCourier.Email,
                             Password = doCourier.Password,
                             IsActive = doCourier.IsActive,
                             MaxDistance = doCourier.PersonalMaxDistance,
                             ShipmentType = (BO.ShipmentType)doCourier.PreferredShipmentType,
                             StartTime = doCourier.EmploymentStartTime,
                         };

        return predicate == null ? boCouriers : boCouriers.Where(predicate);
    }

    

    internal static BO.UserType GetUserType(string id)
    {
        var courier = s_dal.Courier.Read(p => p.Name == id);

        if (courier == null)
        {
            throw new BlDoesNotExistException($"User with Name={id} does not exist");
        }

        return (s_dal.Config.ManagerId == courier.Id) ? BO.UserType.Manager : BO.UserType.Courier;
    }

    #endregion

    #region Setters and Modefiers
    internal static void AddCourier(BO.Courier courier)
    {
   
        try { 
        s_dal.Courier.Create(new DO.Courier()
        {
            Id = courier.Id,
            Name = courier.Name,
            Phone = courier.Phone,
            Email = courier.Email,
            Password = courier.Password,
            IsActive = courier.IsActive,
            PersonalMaxDistance = courier.MaxDistance,
            PreferredShipmentType = (DO.ShipmentType)courier.ShipmentType,
            EmploymentStartTime = courier.StartTime,
        });
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Courier with ID={courier.Id} already exists", ex);
        }
    }

    internal static void DeleteCourier(int id)
    {
        try
        {
            var courier = Helpers.CourierManager.GetCourier(id);
            var hasHistory = s_dal.Delivery.ReadAll(p=> p.CourierId == id);
            if (courier!.InProgress != null)
                throw new BO.BlInvalidOperationStateException("Cannot delete a courier with an order in progress");
            else if(hasHistory.Any())
                throw new BO.BlInvalidOperationStateException("Cannot delete a courier with delivery history");

                s_dal.Courier.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={id} does not exist", ex);
        }
    }

    internal static void UpdateCourier(BO.Courier boCourier)
    {

        // Ensure Validity of Input
        ValidationOfCarrier(boCourier);

        // Check if courier in the middle of a delivery when trying to deactivate
        var activeDeliveriesCheck =
            (from delivery in s_dal.Delivery.ReadAll()
             where delivery.CourierId == boCourier.Id && delivery.EndTime == null
             let isCourierBeingDeactivated = boCourier.IsActive == false
             where isCourierBeingDeactivated
             select delivery).ToList();

        if (activeDeliveriesCheck.Any())
        {
            throw new BO.BlInvalidOperationStateException($"Cannot deactivate courier {boCourier.Id} while they have active deliveries in progress.");
        }

        var courier = s_dal.Courier.Read(boCourier.Id);
   
        // Convert BO to DO
        DO.Courier doCourier = new DO.Courier
        {
            Id = courier.Id == boCourier.Id ? boCourier.Id : throw new BO.BlInvalidInputException("Cannot change id"),
            Name = boCourier.Name,
            Phone = boCourier.Phone,
            Email = boCourier.Email,
            Password = boCourier.Password,
            IsActive = boCourier.IsActive,
            PersonalMaxDistance = boCourier.MaxDistance,
            PreferredShipmentType = (DO.ShipmentType)boCourier.ShipmentType,
            EmploymentStartTime = courier.EmploymentStartTime == boCourier.StartTime? boCourier.StartTime
            : throw new BO.BlInvalidInputException("Cannot change start time"),
        };

        // Update in DAL
        try
        {
            s_dal.Courier.Update(doCourier);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier with ID={boCourier.Id} does not exist", ex);
        }
    }
    #endregion

    #region Validation Methods

    internal static bool EnsureIsManager(string id)
    {
        if (GetUserType(id) != BO.UserType.Manager)
            throw new BLUnauthorizedAccessException("Unauthorized access attempt detected");
        return true;
    }

    /// <summary>
    /// Input Validity Check
    /// </summary>
    /// <returns>true if all valid</returns>
    internal static bool ValidationOfCarrier(BO.Courier courier)
    {
    if(
        Helpers.Tools.IdValidtion(courier.Id)
        || courier.Phone.Length != 10 || courier.Phone[0] != 0
        || Helpers.Tools.IsEmailValidManual(courier.Email)
    ) throw new BO.BlInvalidInputException("Invalid Courier Input");

        return true;
    }

    #endregion
}






