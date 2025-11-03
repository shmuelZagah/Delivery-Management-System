namespace Dal;
using DalApi;
using DO;

public class OrderImplementation : IOrder
{

    public void Create(Order item)
    {

       //creating a copy and adding the item
        }

        //creating a copy and adding the item
        }

        //creating a copy and adding the item
        }

        //creating a copy and adding the item
        Order newOrder = item with { Id = Config.NextOrderId };

        DataSource.orders.Add(newOrder);

    }
    public Order? Read(int id)
    {
        return DataSource.orders.Find(x => x.Id == id);

    }
    public List<Order> ReadAll()
    {
        return new List<Order>(DataSource.orders);
    }
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
    public void Delete(int id)
    {

        Order? toDel = DataSource.orders.Find(x => x.Id == id);
        if (toDel == null)
        {
            throw new Exception($"Order with ID {id} does not exist.");
        }

        else
        {
            DataSource.orders.Remove(toDel);
        }

    }

    public void DeleteAll()
    {
        DataSource.orders.Clear();
    }

}


