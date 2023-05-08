using Employee.WebApi.Application.DbContexts;
using Employee.WebApi.Application.Employees;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Employee.WebApi.Host.Configurations;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection ConfigureDependencyInjection(
        this IServiceCollection services)
    {
        services.AddDbContext<EmployeeContext>(opt => opt.UseInMemoryDatabase("EmployeeDB"));
        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}