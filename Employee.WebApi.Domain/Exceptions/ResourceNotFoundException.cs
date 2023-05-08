using System;

namespace Employee.WebApi.Domain.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message)
        : base(message)
    {
    }
}