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

    public BlDoesNotExistException(string? message , Exception innerException)
       : base(message, innerException)
    {
    }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message)
        : base(message)
    {
    }

    public BlAlreadyExistsException(string? message, Exception innerException)
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

    public BlInvalidInputException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}


/// <summary>
/// Exception for unauthorized access to BL operations.
/// </summary> 
[Serializable]
public class BLUnauthorizedAccessException : Exception
{
    public BLUnauthorizedAccessException(string? message)
        : base(message)
    {
    }

    public BLUnauthorizedAccessException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}


[Serializable]
/// <summary>
/// Thrown when an operation cannot be completed because the entity is currently in a state that does not allow it.
/// </summary> 
public class BlInvalidOperationStateException : Exception
{
    public BlInvalidOperationStateException(string? message)
        : base(message)
    {
    }

    public BlInvalidOperationStateException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}

[Serializable]
/// <summary>
/// Thrown when an operation cannot be performed because the simulator is currently running.
/// </summary> 
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message)
        : base(message)
    {
    }

    public BLTemporaryNotAvailableException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}
