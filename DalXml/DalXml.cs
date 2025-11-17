using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    public class DalXml: IDal
    {
        public ICourier Courier => new CourierImplementation();
        public IOrder Order => new OrderImplementation();
        public IDelivery Delivery => new DeliveryImplementation();
        public IConfig Config => new ConfigImplementation();
        public void ResetDB()
        {
            Delivery.DeleteAll();
            Order.DeleteAll();
            Courier.DeleteAll();
            Config.Reset();
        }
    }
}
