using Employee.WebApi.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace Employee.WebApi.Application.DbContexts;

public class EmployeeContext : DbContext
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options)
        : base(options)
    {
    }

    public DbSet<EmployeeAggregate> Employees { get; set; }
}