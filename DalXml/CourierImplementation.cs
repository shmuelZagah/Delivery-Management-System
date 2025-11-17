namespace Dal;
using DalApi;
using DO;



internal class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        // Check if courier with the same Id already exists
        if (couriers.Exists(courier => courier.Id == item.Id))
            throw new DalAlreadyExistsException($"courier with Id: {item.Id} already  exist");

        //If not, add the new courier to the data source
        couriers.Add(item);

        // Save the updated list back to the XML file
        XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);
    }

    public void Delete(int id)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        Courier? courier = couriers.Find(courier => courier.Id == id);

        // If courier with the specified Id is not found, throw an exception
        if (courier == null)
            throw new DalDoesNotExistException($"courier with Id: {id} dos'nt exist");

        else couriers.Remove(courier);
    }
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Courier>(new List<Courier>(), Config.s_couriers_xml);
    }

    public Courier? Read(int id)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        // return couriers.Find(courier => courier.Id == id);  stage1
        return couriers.FirstOrDefault(courier => courier.Id == id); //stage2
    }
    

    public Courier? Read(Func<Courier, bool> filter) // stage 2
  => DataSource.couriers.FirstOrDefault(deliver => filter(deliver));

    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) //stage 2 
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
