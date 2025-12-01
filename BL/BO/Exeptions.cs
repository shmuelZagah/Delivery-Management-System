using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message)
        : base(message)
    {
    }

    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}


[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message)
        : base(message)
    {
    }

    public BlInvalidInputException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}