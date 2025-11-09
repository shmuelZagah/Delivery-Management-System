
namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? msg) : base(msg) { }
}


[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? msg) : base(msg) { }
}