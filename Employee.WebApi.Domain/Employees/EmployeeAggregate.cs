using System;

namespace Employee.WebApi.Domain.Employees;

public class EmployeeAggregate : IAggregateRoot
{
    public EmployeeAggregate(Guid id,
        string firstName,
        string lastName,
        DateTime birthDate,
        DateTime employmentDate,
        string homeAddress,
        decimal currentSalary,
        EmployeeRole role,
        Guid? bossId
        )
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        EmploymentDate = employmentDate;
        HomeAddress = homeAddress;
        CurrentSalary = currentSalary;
        Role = role;
        BossId = bossId;
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateTime BirthDate { get; private set; }

    public DateTime EmploymentDate { get; private set; }

    public string HomeAddress { get; private set; }

    public decimal CurrentSalary { get; private set; }

    public EmployeeRole Role { get; private set; }

    public Guid? BossId { get; private set; }

    public void Update(string firstName,
        string lastName,
        DateTime birthDate,
        DateTime employmentDate,
        string homeAddress,
        decimal currentSalary,
        EmployeeRole role,
        Guid? bossId)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        EmploymentDate = employmentDate;
        HomeAddress = homeAddress;
        CurrentSalary = currentSalary;
        Role = role;
        BossId = bossId;
    }
}