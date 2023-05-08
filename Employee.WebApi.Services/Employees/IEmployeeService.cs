using Employee.WebApi.Contracts.Employees;
using System;
using System.Threading.Tasks;

namespace Employee.WebApi.Application.Employees;

public interface IEmployeeService
{
    Task<EmployeeCreated> CreateAsync(CreateEmployee createEmployee);

    Task<Contracts.Employees.Employee> GetAsync(Guid id);

    Task UpdateAsync(Guid id, UpdateEmployee update);

    Task DeleteAsync(Guid id);
}