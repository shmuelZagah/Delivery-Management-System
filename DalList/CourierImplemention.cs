namespace Dal;
using DalApi;
using DO;

public class CourierImplemention : ICourier
{
    public void Create(Courier item) 
    {
    }

    public void Delete(int id) =>  throw new NotImplementedException();

    public void DeleteAll() => throw new NotImplementedException();
    public Courier? Read(int id) => throw new NotImplementedException();
    public List<Courier> ReadAll() => throw new NotImplementedException();
    public void Update(Courier item) => throw new NotImplementedException();
}
