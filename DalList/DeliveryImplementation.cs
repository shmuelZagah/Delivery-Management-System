namespace Dal;
using DalApi;
using DO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

internal class DeliveryImplementation : IDelivery
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        
        //create new delivery with new ID
        Delivery delivery = item with { Id = Config.NextDeliveryId };

        DataSource.deliveries.Add(delivery);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Delivery? delivery = DataSource.deliveries.Find(deliver => deliver.Id == id);
        if (delivery == null)
        {
            throw new DalDoesNotExistException($"Delivery with ID {id} does not exist.");
        }
        else
        {
            DataSource.deliveries.Remove(delivery);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll() //stage1
    {
        DataSource.deliveries.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {
        //return DataSource.deliveries.Find(deliver => deliver.Id == id); stage1
        return DataSource.deliveries.FirstOrDefault(deliver => deliver.Id == id);
    }

    //public List<Delivery> ReadAll()
    //{
    //    return new List<Delivery>(DataSource.deliveries);
    //}

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter) // stage 2
    => DataSource.deliveries.FirstOrDefault(deliver => filter(deliver));

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) //stage 2
     => filter != null
         ? from item in DataSource.deliveries
           where filter(item)
           select item
          : from item in DataSource.deliveries
            select item;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        int index = DataSource.deliveries.FindIndex(deliver => deliver.Id == item.Id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Delivery with ID {item.Id} does not exist.");
        }
        else
        {
            DataSource.deliveries[index] = item;
        }
    }
}
