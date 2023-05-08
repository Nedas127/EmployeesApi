using Employee.WebApi.Contracts.Employees;
using Employee.WebApi.Domain.Employees;
using FluentValidation;
using System;

namespace Employee.WebApi.Application.Employees;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployee>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.BirthDate).NotEmpty();
        RuleFor(x => x.EmploymentDate).NotEmpty();
        RuleFor(x => x.HomeAddress).NotEmpty();
        RuleFor(x => x.CurrentSalary).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().Must(x => Enum.TryParse<EmployeeRole>(x, out _));
        RuleFor(x => x.BossId).Empty().When(x => x.Role == EmployeeRole.Ceo.ToString());
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(50);
        RuleFor(x => x.FirstName).NotEqual(x => x.LastName);
        RuleFor(x => x.BirthDateAsDateTime)
            .NotEmpty()
            .ExclusiveBetween(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-18))
            .OverridePropertyName(nameof(CreateEmployee.BirthDate));
        RuleFor(x => x.EmploymentDate).GreaterThan(DateTime.Parse("2000-01-01"));
        RuleFor(x => x.EmploymentDate).LessThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.CurrentSalary).GreaterThanOrEqualTo(0);
    }
}

// TODO: Refactor to not duplicate validation code
public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployee>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.BirthDate).NotEmpty();
        RuleFor(x => x.EmploymentDate).NotEmpty();
        RuleFor(x => x.HomeAddress).NotEmpty();
        RuleFor(x => x.CurrentSalary).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().Must(x => Enum.TryParse<EmployeeRole>(x, out _));
        RuleFor(x => x.BossId).Empty().When(x => x.Role == EmployeeRole.Ceo.ToString());
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(50);
        RuleFor(x => x.FirstName).NotEqual(x => x.LastName);
        RuleFor(x => x.BirthDateAsDateTime)
            .NotEmpty()
            .ExclusiveBetween(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-18))
            .OverridePropertyName(nameof(CreateEmployee.BirthDate));
        RuleFor(x => x.EmploymentDate).GreaterThan(DateTime.Parse("2000-01-01"));
        RuleFor(x => x.EmploymentDate).LessThanOrEqualTo(DateTime.Today);
    }
}