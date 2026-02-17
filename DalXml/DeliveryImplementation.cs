namespace Dal;
using DalApi;
using DO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class DeliveryImplementation : IDelivery
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        XElement? deliveryRootElemnt = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

        //create new delivery with new ID
        Delivery delivery = item with { Id = Config.NextDeliveryId };

        //add delivery to xml
        deliveryRootElemnt.Add(
            new XElement("Delivery",
            new XElement("Id", delivery.Id),
            new XElement("OrderId", delivery.OrderId),
            new XElement("CourierId", delivery.CourierId),
            new XElement("DeliveryType", delivery.DeliveryType),
            new XElement("StartTime", delivery.StartTime),
            new XElement("DistanceKm", delivery.DistanceKm),
            new XElement("DeliveryEndType", delivery.DeliveryEndType),
            new XElement("EndTime", delivery.EndTime)
        ));

        //save to xml
        XMLTools.SaveListToXMLElement(deliveryRootElemnt, Config.s_deliverys_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        //Load elemnts from xml
        XElement deliveryRootElement = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

        (deliveryRootElement.Elements().FirstOrDefault(
            delivery => int.Parse(delivery.Element("Id")!.Value) == id) ??
            throw new DalDoesNotExistException($"Delivery with ID {id} does not exist.")).Remove();

        XMLTools.SaveListToXMLElement(deliveryRootElement, Config.s_deliverys_xml);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll() 
    {
        XElement deliveryRootElement = new XElement("Deliveries");

        XMLTools.SaveListToXMLElement(deliveryRootElement, Config.s_deliverys_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {
        XElement deliveryRootElement = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

        //return DataSource.deliveries.Find(deliver => deliver.Id == id); stage1
        XElement? deliveryElement = deliveryRootElement.Elements().FirstOrDefault(delivery => int.Parse(delivery.Element("Id")!.Value) == id);

        if (deliveryElement == null) return null;

        Delivery delivery = new Delivery
        {
            
            Id = XMLTools.ToIntNullable(deliveryElement,"Id") ?? 0,
            OrderId = XMLTools.ToIntNullable(deliveryElement,"OrderId") ?? 0,
            CourierId = XMLTools.ToIntNullable(deliveryElement,"CourierId") ?? 0,
            DeliveryType = XMLTools.ToEnumNullable<ShipmentType>(deliveryElement,"DeliveryType") ?? 0,
            StartTime = XMLTools.ToDateTimeNullable(deliveryElement,"StartTime") ?? DateTime.Now,
            DistanceKm = XMLTools.ToDoubleNullable(deliveryElement,"DistanceKm") ?? 0,
            DeliveryEndType = XMLTools.ToEnumNullable<DeliveryEndType>(deliveryElement,"DeliveryEndType") ?? null,
            EndTime = XMLTools.ToDateTimeNullable(deliveryElement,"EndTime") ?? null
        };

        return delivery;
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        XElement rootDelivery = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

        foreach (var deliveryElement in rootDelivery.Elements())
        {
            Delivery delivery = new Delivery
            {
                Id = XMLTools.ToIntNullable(deliveryElement, "Id") ?? 0,
                OrderId = XMLTools.ToIntNullable(deliveryElement, "OrderId") ?? 0,
                CourierId = XMLTools.ToIntNullable(deliveryElement, "CourierId") ?? 0,
                DeliveryType = XMLTools.ToEnumNullable<ShipmentType>(deliveryElement, "DeliveryType") ?? 0,
                StartTime = XMLTools.ToDateTimeNullable(deliveryElement, "StartTime") ?? DateTime.Now,
                DistanceKm = XMLTools.ToDoubleNullable(deliveryElement, "DistanceKm") ?? 0,
                DeliveryEndType = string.IsNullOrEmpty(deliveryElement.Element("DeliveryEndType")?.Value)
                  ? null
                  : XMLTools.ToEnumNullable<DeliveryEndType>(deliveryElement, "DeliveryEndType"),
                EndTime = XMLTools.ToDateTimeNullable(deliveryElement, "EndTime") ?? null
            };
            if (filter(delivery)) return delivery;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        XElement? deliveryRoot = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

        // הופכים את כל ה-XML לרשימת אובייקטים פעם אחת בלבד
        var allDeliveries = from deliveryElement in deliveryRoot.Elements()
                            select new Delivery
                            (
                                XMLTools.ToIntNullable(deliveryElement, "Id") ?? 0,
                                XMLTools.ToIntNullable(deliveryElement, "OrderId") ?? 0,
                                XMLTools.ToIntNullable(deliveryElement, "CourierId") ?? 0,
                                XMLTools.ToEnumNullable<ShipmentType>(deliveryElement, "DeliveryType") ?? 0,
                                XMLTools.ToDateTimeNullable(deliveryElement, "StartTime") ?? DateTime.Now,
                                XMLTools.ToDoubleNullable(deliveryElement, "DistanceKm"),
                                XMLTools.ToEnumNullable<DeliveryEndType>(deliveryElement, "DeliveryEndType"),
                                XMLTools.ToDateTimeNullable(deliveryElement, "EndTime")
                            );

        // רק בסוף מפעילים את הפילטר על הרשימה המוכנה
        return filter == null ? allDeliveries.ToList() : allDeliveries.Where(filter).ToList();
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        XElement root = XMLTools.LoadListFromXMLElement(Config.s_deliverys_xml);

      
        XElement? deliveryElem = root.Elements()
            .FirstOrDefault(d => XMLTools.ToIntNullable(d, "Id") == item.Id);

        if (deliveryElem == null)
            throw new DalDoesNotExistException($"Delivery with ID {item.Id} does not exist.");

     
        deliveryElem.SetElementValue("OrderId", item.OrderId);
        deliveryElem.SetElementValue("CourierId", item.CourierId);
        deliveryElem.SetElementValue("DeliveryType", item.DeliveryType);
        deliveryElem.SetElementValue("StartTime", item.StartTime);
        deliveryElem.SetElementValue("DistanceKm", item.DistanceKm);
        deliveryElem.SetElementValue("DeliveryEndType", item.DeliveryEndType);
        deliveryElem.SetElementValue("EndTime", item.EndTime);

 
        XMLTools.SaveListToXMLElement(root, Config.s_deliverys_xml);
    }


}



