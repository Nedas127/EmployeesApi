using System;

namespace Employee.WebApi.Contracts.Employees;

public record Employee : ModifiableEmployeeData
{
    public Guid Id { get; init; }
}