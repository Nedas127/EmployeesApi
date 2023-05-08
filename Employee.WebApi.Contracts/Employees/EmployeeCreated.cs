using System;

namespace Employee.WebApi.Contracts.Employees;

public record EmployeeCreated
{
    public Guid Id { get; init; }
}