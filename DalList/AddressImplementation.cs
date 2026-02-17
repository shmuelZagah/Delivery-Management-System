namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

internal class AddressImplementation : IAddress
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Address item)
    {
        if (DataSource.addresses.Exists(Address => Address.Id == item.Id)) 
            throw new DO.DalAlreadyExistsException($"The Address: {item} is already exist");

        DataSource.addresses.Add(item with { Id = Config.NextAddressId});
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var addr = DataSource.addresses.Find(ad => ad.Id == id);

        if (addr == null)
            throw new DO.DalDoesNotExistException($"Address with id: {id} is not exist");

        DataSource.addresses.Remove(addr);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.addresses.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Address? Read(int id)
    => DataSource.addresses.FirstOrDefault(a => a.Id == id);
    

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Address? Read(Func<Address, bool> filter) =>
     DataSource.addresses.FirstOrDefault(adr => filter(adr));


    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Address> ReadAll(Func<Address, bool>? filter = null)
   => filter != null ?
        from item in DataSource.addresses
        where filter(item)
        select item
        :
        from item in DataSource.addresses
        select item;


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Address item)
    {
        int index =DataSource.addresses.FindIndex(adr => adr.Id == item.Id);
        if(index == -1)
            throw new DO.DalDoesNotExistException($"Address with id: {item.Id} is not exist");

        DataSource.addresses[index] = item;
    }
}
