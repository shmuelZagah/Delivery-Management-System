using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApi
{
    public interface ICourier
    {
        UserType Login(string username, string password);

        IEnumerable<BO.Courier> getCouriers(int requesterId, bool? isActive, CourierField? courierField);

        BO.Courier GetCourierDetails(int requesterId, int courierId);

        void updateCourier(int requesterId, BO.Courier courier);

        void deleteCourier(int requesterId, int courierId);

        void addCourier(int requesterId,  BO.Courier courier);
    }
}
