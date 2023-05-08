using System;

namespace Employee.WebApi.Contracts.Employees;

public abstract record ModifiableEmployeeData
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string BirthDate { get; init; }

    public DateTime? BirthDateAsDateTime => DateTime.TryParse(BirthDate, out var val) ? val : null;

    public DateTime EmploymentDate { get; init; }

    public string HomeAddress { get; init; }

    public decimal CurrentSalary { get; init; }

    public string Role { get; init; }

    public Guid? BossId { get; init; }
}