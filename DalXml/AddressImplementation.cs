using DalApi;
using DO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal;

internal class AddressImplementation : IAddress
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Address item)
    {
        XElement? root = XMLTools.LoadListFromXMLElement(Config.s_address_xml);

        Address adr = item with { Id = Config.NextAddressId };

        root.Add(
            new XElement("Address",

            new XElement("Id", adr.Id),
            new XElement("FullAddress", adr.FullAddress),
            new XElement("Latitude", adr.Latitude),
            new XElement("Longitude", adr.Longitude)
            ));

        //save to xml
        XMLTools.SaveListToXMLElement(root, Config.s_address_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement root = XMLTools.LoadListFromXMLElement(Config.s_address_xml);

        (
            root.Elements().FirstOrDefault(
                d => (int?)d.Element("Id") == id) ??
            throw new DO.DalDoesNotExistException($"Address with id: {id} dosnt exist")
            )
            .Remove();

        XMLTools.SaveListToXMLElement(root, Config.s_address_xml);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll() =>
        XMLTools.SaveListToXMLElement(new XElement("ArrayOfAddress"), Config.s_address_xml);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Address? Read(int id)
    {
        XElement root = XMLTools.LoadListFromXMLElement(Config.s_address_xml);

        var adrElement = root.Elements().FirstOrDefault
            (d => (int?)d.Element("Id") == id);

        if (adrElement == null) return null;

        Address adr = new Address(
            id,
            (string?)adrElement.Element("FullAddress") ?? string.Empty,
            XMLTools.ToDoubleNullable(adrElement, "Latitude") ?? 0.0,
            XMLTools.ToDoubleNullable(adrElement, "Longitude") ?? 0.0
        );

        return adr;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Address? Read(Func<Address, bool> filter)
    {
        XElement root = XMLTools.LoadListFromXMLElement (Config.s_address_xml);

        var addresses = root.Elements().Select(adr => new Address(
          (int?)adr.Element("Id") ?? -1,
          (string?)adr.Element("FullAddress") ?? "",
          (double?)adr.Element("Latitude") ?? 0.0,
          (double?)adr.Element("Longitude") ?? 0.0
      ));

        var adr = addresses.FirstOrDefault(a => filter(a));
        if (adr == null) return null;

        return adr;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Address> ReadAll(Func<Address, bool>? filter = null)
    {
        XElement root = XMLTools.LoadListFromXMLElement(Config.s_address_xml);

        var addresses = root.Elements().Select(adr => new Address(
            (int?)adr.Element("Id") ?? -1, 
            (string?)adr.Element("FullAddress") ?? "",
            (double?)adr.Element("Latitude") ?? 0.0,
            (double?)adr.Element("Longitude") ?? 0.0
        ));

        if (filter != null)
        {
            return addresses.Where(filter);
        }

        return addresses;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Address item)
    {
        XElement root = XMLTools.LoadListFromXMLElement(Config.s_address_xml);

        XElement? addrElement = root.Elements().FirstOrDefault(d => XMLTools.ToIntNullable(d, "Id") == item.Id);

        if (addrElement == null)
            throw new DO.DalDoesNotExistException($"Address with id: {item.Id} dosnt exist");


        addrElement.SetElementValue("Id", item.Id);
        addrElement.SetElementValue("FullAddress", item.FullAddress);
        addrElement.SetElementValue("Latitude", item.Latitude);
        addrElement.SetElementValue("Longitude", item.Longitude);

        XMLTools.SaveListToXMLElement(root, Config.s_address_xml);
    }
}
