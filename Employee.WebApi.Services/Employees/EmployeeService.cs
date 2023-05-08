using Employee.WebApi.Application.DbContexts;
using Employee.WebApi.Contracts.Employees;
using Employee.WebApi.Domain.Employees;
using Employee.WebApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Employee.WebApi.Application.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeContext _employeeContext;

    public EmployeeService(EmployeeContext employeeContext)
    {
        _employeeContext = employeeContext;
    }

    public async Task<EmployeeCreated> CreateAsync(CreateEmployee createEmployee)
    {
        var aggregate = new EmployeeAggregate(Guid.NewGuid(), createEmployee.FirstName, createEmployee.LastName,
            DateTime.Parse(createEmployee.BirthDate), createEmployee.EmploymentDate, createEmployee.HomeAddress,
            createEmployee.CurrentSalary, Enum.Parse<EmployeeRole>(createEmployee.Role), createEmployee.BossId);

        await ValidateAggregate(aggregate);

        _employeeContext.Employees.Add(aggregate);
        await _employeeContext.SaveChangesAsync();

        return new EmployeeCreated
        {
            Id = aggregate.Id
        };
    }

    public async Task<Contracts.Employees.Employee> GetAsync(Guid id)
    {
        return await _employeeContext.Employees.Select(o => new Contracts.Employees.Employee
        {
            Id = o.Id,
            FirstName = o.FirstName,
            LastName = o.LastName,
            BirthDate = o.BirthDate.ToShortDateString(),
            EmploymentDate = o.EmploymentDate,
            HomeAddress = o.HomeAddress,
            CurrentSalary = o.CurrentSalary,
            Role = o.Role.ToString(),
            BossId = o.BossId
        }).FirstOrDefaultAsync(o => o.Id == id) ?? throw new ResourceNotFoundException("Employees not found");
    }

    public async Task UpdateAsync(Guid id, UpdateEmployee update)
    {
        var aggregate = await _employeeContext.Employees.FirstOrDefaultAsync(o => o.Id == id) ??
                        throw new ResourceNotFoundException("Employees not found");

        await ValidateAggregate(aggregate);

        aggregate.Update(update.FirstName, update.LastName, DateTime.Parse(update.BirthDate), update.EmploymentDate,
            update.HomeAddress, update.CurrentSalary, Enum.Parse<EmployeeRole>(update.Role), update.BossId);
        await _employeeContext.SaveChangesAsync();
    }

    private async Task ValidateAggregate(EmployeeAggregate aggregate)
    {
        if (aggregate.Role == EmployeeRole.Ceo &&
            await _employeeContext.Employees.AnyAsync(o => o.Role == EmployeeRole.Ceo))
        {
            throw new BadRequestException("There can be only 1 employee with CEO role");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var aggregate = await _employeeContext.Employees.FirstOrDefaultAsync(o => o.Id == id) ??
                        throw new ResourceNotFoundException("Employees not found");

        _employeeContext.Employees.Remove(aggregate);
        await _employeeContext.SaveChangesAsync();
    }
}