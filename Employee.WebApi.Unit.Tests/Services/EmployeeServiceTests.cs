using Employee.WebApi.Application.DbContexts;
using Employee.WebApi.Application.Employees;
using Employee.WebApi.Contracts.Employees;
using Employee.WebApi.Domain.Employees;
using Employee.WebApi.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Employee.WebApi.Unit.Tests.Services;

public class EmployeeServiceTests
{
    private EmployeeContext _employeeContext;
    private IEmployeeService _employeeService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EmployeeContext>()
            .UseInMemoryDatabase(databaseName: "EmployeeDB")
            .Options;

        _employeeContext = new EmployeeContext(options);
        _employeeService = new EmployeeService(_employeeContext);
    }

    [TearDown]
    public void TearDown()
    {
        _employeeContext.Database.EnsureDeleted();
    }

    [Test]
    public async Task CreateAsync_ValidInput_ReturnsEmployeeCreated()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now,
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _employeeService.CreateAsync(createEmployee);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Id);
        Assert.AreEqual(1, _employeeContext.Employees.Count());
    }

    [Test]
    public void CreateAsync_InvalidRole_ThrowsException()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 100000,
            Role = "InvalidRole",
            BossId = Guid.NewGuid()
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await _employeeService.CreateAsync(createEmployee));
        Assert.AreEqual(0, _employeeContext.Employees.Count());
    }

    [Test]
    public async Task GetAsync_ValidInput_ReturnsEmployee()
    {
        var employee = new EmployeeAggregate(
            Guid.NewGuid(),
            "John",
            "Doe",
            DateTime.Parse("1990-01-01"),
            DateTime.Now,
            "123 Main St, Anytown USA",
            50000,
            EmployeeRole.AccountManager,
            Guid.NewGuid());
        _employeeContext.Employees.Add(employee);
        await _employeeContext.SaveChangesAsync();

        var result = await _employeeService.GetAsync(employee.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(employee.Id, result.Id);
        Assert.AreEqual(employee.FirstName, result.FirstName);
        Assert.AreEqual(employee.LastName, result.LastName);
        Assert.AreEqual(employee.BirthDate.ToShortDateString(), result.BirthDate);
        Assert.AreEqual(employee.EmploymentDate, result.EmploymentDate);
        Assert.AreEqual(employee.HomeAddress, result.HomeAddress);
        Assert.AreEqual(employee.CurrentSalary, result.CurrentSalary);
        Assert.AreEqual(employee.Role.ToString(), result.Role);
        Assert.AreEqual(employee.BossId, result.BossId);
    }

    [Test]
    public void GetAsync_EmployeeByIdNotFound_ThrowsResourceNotFoundException()
    {
        var nonExistentEmployeeId = Guid.NewGuid();

        Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _employeeService.GetAsync(nonExistentEmployeeId));
    }

    [Test]
    public async Task UpdateAsync_ValidInput_UpdatesEmployee()
    {
        var employee = new EmployeeAggregate(
            Guid.NewGuid(),
            "John",
            "Doe",
            DateTime.Parse("1990-01-01"),
            DateTime.Now,
            "123 Main St, Anytown USA",
            50000,
            EmployeeRole.AccountManager,
            Guid.NewGuid());
        _employeeContext.Employees.Add(employee);
        await _employeeContext.SaveChangesAsync();
        var updateEmployee = new UpdateEmployee
        {
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = "1992-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "456 Elm St, Anytown USA",
            CurrentSalary = 60000,
            Role = EmployeeRole.Developer.ToString(),
            BossId = Guid.NewGuid()
        };

        await _employeeService.UpdateAsync(employee.Id, updateEmployee);
        var updatedEmployee = await _employeeContext.Employees.FindAsync(employee.Id);

        Assert.AreEqual(updateEmployee.FirstName, updatedEmployee.FirstName);
        Assert.AreEqual(updateEmployee.LastName, updatedEmployee.LastName);
        Assert.AreEqual(DateTime.Parse(updateEmployee.BirthDate), updatedEmployee.BirthDate);
        Assert.AreEqual(updateEmployee.EmploymentDate, updatedEmployee.EmploymentDate);
        Assert.AreEqual(updateEmployee.HomeAddress, updatedEmployee.HomeAddress);
        Assert.AreEqual(updateEmployee.CurrentSalary, updatedEmployee.CurrentSalary);
        Assert.AreEqual(Enum.Parse<EmployeeRole>(updateEmployee.Role), updatedEmployee.Role);
        Assert.AreEqual(updateEmployee.BossId, updatedEmployee.BossId);
    }

    [Test]
    public void UpdateAsync_InvalidId_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        var updateEmployee = new UpdateEmployee
        {
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "456 Elm St, Anytown USA",
            CurrentSalary = 60000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _employeeService.UpdateAsync(id, updateEmployee));
    }

    [Test]
    public async Task DeleteAsync_ValidInput_RemovesEmployee()
    {
        var employee = new EmployeeAggregate(
            Guid.NewGuid(),
            "John",
            "Doe",
            DateTime.Parse("1990-01-01"),
            DateTime.Now,
            "123 Main St, Anytown USA",
            50000,
            EmployeeRole.AccountManager,
            Guid.NewGuid());
        _employeeContext.Employees.Add(employee);
        await _employeeContext.SaveChangesAsync();

        await _employeeService.DeleteAsync(employee.Id);

        Assert.AreEqual(0, _employeeContext.Employees.Count());
    }
}