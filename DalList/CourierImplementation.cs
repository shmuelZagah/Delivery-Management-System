namespace Dal;
using DalApi;
using DO;

internal class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        // Check if courier with the same Id already exists
        if (DataSource.couriers.Exists(courier => courier.Id == item.Id))
            throw new DalAlreadyExistsException($"courier with Id: {item.Id} already  exist");

        //If not, add the new courier to the data source
        DataSource.couriers.Add(item);
    }

    public void Delete(int id)
    {
        Courier? courier = DataSource.couriers.Find(courier => courier.Id == id);

        // If courier with the specified Id is not found, throw an exception
        if (courier == null) 
            throw new DalDoesNotExistException($"courier with Id: {id} dos'nt exist");

        else DataSource.couriers.Remove(courier);
    }
    public void DeleteAll()
    {
        DataSource.couriers.Clear();
    }

    public Courier? Read(int id) 
    {
        // return DataSource.couriers.Find(courier => courier.Id == id);  stage1
        return DataSource.couriers.FirstOrDefault(courier => courier.Id == id); 
    }
    

    public Courier? Read(Func<Courier, bool> filter) 
  => DataSource.couriers.FirstOrDefault(deliver => filter(deliver));

    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) 
         => filter != null
             ? from item in DataSource.couriers
               where filter(item)
               select item
              : from item in DataSource.couriers
              select item; 
    

    public void Update(Courier item)
    {
        // Search for the courier with the specified Id
        int index = DataSource.couriers.FindIndex(courier => courier.Id == item.Id);

        //If courier not found , throw exception
        if (index == -1)
        {
            throw new DalDoesNotExistException($"courier with Id: {item.Id} dos'nt exist");
        }

        // If found, remove the old courier and add the updated one
        else
        {
            DataSource.couriers[index] = item;
        }
    }
}
