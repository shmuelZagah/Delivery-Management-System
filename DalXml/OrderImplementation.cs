namespace Dal;
using DO;
using DalApi;
using System;
using System.Collections.Generic;

internal class OrderImplementation : IOrder
{
    public void Create(Order item)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        //If not, add the new courier to the data source
        orders.Add(item with {Id= Config.NextOrderId });

        // Save the updated list back to the XML file
        XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
    }

    public void Delete(int id)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        Order? order = orders.Find(courier => courier.Id == id);

        // If courier with the specified Id is not found, throw an exception
        if (order == null)
            throw new DalDoesNotExistException($"courier with Id: {id} dos'nt exist");

        else orders.Remove(order);
    }
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Order>(new List<Order>(), Config.s_orders_xml);
    }

    public Order? Read(int id)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        return orders.FirstOrDefault(order => order.Id == id);
    }


    public Order? Read(Func<Order, bool> filter)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        return orders.FirstOrDefault(filter);
    }


    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        return
            //if there is a function filter , return orders that match the filter
            filter != null
             ? from item in orders
               where filter(item)
               select item

              //if there is no function filter , return all orders
              : from item in orders
                select item;

    }



    public void Update(Order item)
    {
        //Load data from the data source
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);

        // Search for the courier with the specified Id
        int index = orders.FindIndex(courier => courier.Id == item.Id);

        //If courier not found , throw exception
        if (index == -1)
        {
            throw new DalDoesNotExistException($"courier with Id: {item.Id} dos'nt exist");
        }

        // If found, remove the old courier and add the updated one
        else
        {
            orders[index] = item;
            // Save the updated list back to the XML file
            XMLTools.SaveListToXMLSerializer<Order>(orders, Config.s_orders_xml);
        }
    }
}
