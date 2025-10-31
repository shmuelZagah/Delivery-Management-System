namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class DeliveryImplemention : IDelivery
{
    public void Create(Delivery item)
    {
        //check if delivery with same ID already exists
        if (DataSource.deliveries.Exists(deliver => deliver.Id == item.Id))
            throw new Exception($"Delivery with ID {item.Id} already exists.");

        //create new delivery with new ID
        Delivery delivery = item with { Id = Config.NextDeliveryId };

        DataSource.deliveries.Add(delivery);
    }

    public void Delete(int id)
    {
        Delivery? delivery = DataSource.deliveries.Find(deliver => deliver.Id == id);
        if (delivery == null)
        {
            throw new Exception($"Delivery with ID {id} does not exist.");
        }
        else
        {
            DataSource.deliveries.Remove(delivery);
        }
    }

    public void DeleteAll()
    {
        DataSource.deliveries.Clear();
    }

    public Delivery? Read(int id)
    {
        return DataSource.deliveries.Find(deliver => deliver.Id == id);
    }

    public List<Delivery> ReadAll()
    {
        return new List<Delivery>(DataSource.deliveries);
    }

    public void Update(Delivery item)
    {
        int index = DataSource.deliveries.FindIndex(deliver => deliver.Id == item.Id);
        if (index == -1)
        {
            throw new Exception($"Delivery with ID {item.Id} does not exist.");
        }
        else
        {
            DataSource.deliveries[index] = item;
        }
    }
}
