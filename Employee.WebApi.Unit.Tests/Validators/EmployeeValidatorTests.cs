using Employee.WebApi.Application.Employees;
using Employee.WebApi.Contracts.Employees;
using Employee.WebApi.Domain.Employees;
using FluentValidation.TestHelper;

namespace Employee.WebApi.Unit.Tests.Validators;

public class EmployeeValidatorTests
{
    private CreateEmployeeValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateEmployeeValidator();
    }

    [Test]
    public async Task ValidateAsync_AllParametersAreValid_ReturnsValid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ValidateAsync_EmptyFirstName_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };
        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorCode("NotEmptyValidator");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_FirstNameEqualToLastName_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "John",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };
        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorCode("NotEqualValidator")
            .WithErrorMessage($"'First Name' must not be equal to '{createEmployee.LastName}'.");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_CeoRoleWithBoss_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.Ceo.ToString(),
            BossId = Guid.NewGuid()
        };
        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.BossId)
            .WithErrorCode("EmptyValidator")
            .WithErrorMessage("'Boss Id' must be empty.");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_FirstNameTooLong_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = new string('a', 51),
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(-7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorCode("MaximumLengthValidator")
            .WithErrorMessage($"The length of 'First Name' must be 50 characters or fewer. You entered {createEmployee.FirstName.Length} characters.");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_FutureEmploymentDate_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Now.AddDays(7),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.EmploymentDate)
            .WithErrorCode("LessThanOrEqualValidator")
            .WithErrorMessage($"'Employment Date' must be less than or equal to '{DateTime.Now.Date}'.");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_EarlierEmploymentDate_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Parse("2000-01-01"),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = 50000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.EmploymentDate)
            .WithErrorCode("GreaterThanValidator")
            .WithErrorMessage($"'Employment Date' must be greater than '{createEmployee.EmploymentDate}'.");
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ValidateAsync_NegativeCurrentSalary_ReturnsInvalid()
    {
        var createEmployee = new CreateEmployee
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = "1990-01-01",
            EmploymentDate = DateTime.Parse("2000-01-01"),
            HomeAddress = "123 Main St, Anytown USA",
            CurrentSalary = -3000,
            Role = EmployeeRole.AccountManager.ToString(),
            BossId = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(createEmployee);

        result.ShouldHaveValidationErrorFor(x => x.CurrentSalary)
            .WithErrorCode("GreaterThanOrEqualValidator")
            .WithErrorMessage($"'Current Salary' must be greater than or equal to '0'.");
        Assert.IsFalse(result.IsValid);
    }
}