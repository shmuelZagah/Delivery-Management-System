using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dal;

using DalApi;
using DO;

public class OrderImplementation : IOrder
{

    public void Create(Order item)
    {
         bool isExist = false;

         foreach(var i = )
    }
    public Order? Read(int id) => throw new NotImplementedException();
    public List<Order> ReadAll() => throw new NotImplementedException();
    public void Update(Order item) => throw new NotImplementedException();
    public void Delete(int id) => throw new NotImplementedException();
    public void DeleteAll() => throw new NotImplementedException();

}


