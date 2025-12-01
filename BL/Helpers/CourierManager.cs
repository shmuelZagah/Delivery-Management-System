using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;

    internal static BO.Courier? GetCourier(int id)
    {
        DO.Courier? doCourier = null;

        doCourier = s_dal.Courier.Read(id) ?? 
            throw new Exception /*BO.BlDoesNotExistException*/ ($"Courier with ID={id} does Not exist");



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



}

 



