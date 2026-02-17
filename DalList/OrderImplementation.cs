namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

internal class OrderImplementation : IOrder
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {

        //creating a copy and adding the item
        Order newOrder = item with { Id = Config.NextOrderId };
        DataSource.orders.Add(newOrder);

    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {
        //return DataSource.orders.Find(x => x.Id == id); stage1
        return DataSource.orders.FirstOrDefault(order => order.Id == id); //stage2

    }

    /* stage1
      public List<Order> ReadAll()
    {
        return new List<Order>(DataSource.orders);
    }
     */
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter) // stage 2
  => DataSource.orders.FirstOrDefault(deliver => filter(deliver));

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) //stage 2 
  => filter != null
      ? from item in DataSource.orders
        where filter(item)
        select item
       : from item in DataSource.orders
         select item;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        int index = DataSource.orders.FindIndex(deliver => deliver.Id == item.Id);
        if (index == -1)
        {
            throw new Exception($"Order with ID {item.Id} does not exist.");
        }
        else
        {
            DataSource.orders[index] = item;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {

        Order? toDel = DataSource.orders.Find(x => x.Id == id);
        if (toDel == null)
        {
            throw new DalDoesNotExistException($"Order with ID {id} does not exist.");
        }

        else
        {
            DataSource.orders.Remove(toDel);
        }

    }

    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        DataSource.orders.Clear();
    }

}


