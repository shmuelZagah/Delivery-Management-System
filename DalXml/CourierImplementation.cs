namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

internal class CourierImplementation : ICourier
{

    [MethodImpl(MethodImplOptions.Synchronized)]
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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        Courier? courier = couriers.Find(courier => courier.Id == id);

        // If courier with the specified Id is not found, throw an exception
        if (courier == null)
            throw new DalDoesNotExistException($"courier with Id: {id} dos'nt exist");

        else
        {
            couriers.Remove(courier);
            XMLTools.SaveListToXMLSerializer(couriers, Config.s_couriers_xml);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Courier>(new List<Courier>(), Config.s_couriers_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        return couriers.FirstOrDefault(courier => courier.Id == id); 
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(Func<Courier, bool> filter)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        return couriers.FirstOrDefault(deliver => filter(deliver));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        return
            //if there is a function filter , return couriers that match the filter
            filter != null
             ? from item in couriers
               where filter(item)
               select item

              //if there is no function filter , return all couriers
              : from item in couriers
                select item;

    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        //Load data from the data source
        List<Courier> couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);

        // Search for the courier with the specified Id
        int index = couriers.FindIndex(courier => courier.Id == item.Id);

        //If courier not found , throw exception
        if (index == -1)
        {
            throw new DalDoesNotExistException($"courier with Id: {item.Id} dos'nt exist");
        }

        // If found, remove the old courier and add the updated one
        else
        {
            couriers[index] = item;
            // Save the updated list back to the XML file
            XMLTools.SaveListToXMLSerializer<Courier>(couriers, Config.s_couriers_xml);
        }
    }
}
