using Employee.WebApi.Application.Employees;
using Employee.WebApi.Contracts.Employees;
using Employee.WebApi.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Employee.WebApi.Host.Controllers;

[ApiController]
[Route("v1/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    ///     Creates a new employee
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeCreated), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateEmployee createEmployee)
    {
        var employeeCreated = await _employeeService.CreateAsync(createEmployee);

        return CreatedAtAction(
            nameof(Get),
            new
            {
                employeeCreated.Id
            },
            employeeCreated);
    }

    /// <summary>
    ///     Returns details of an employee
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Contracts.Employees.Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var employee = await _employeeService.GetAsync(id);

        return Ok(employee);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> JsonPatchWithModelState(Guid id, [FromBody] JsonPatchDocument<UpdateEmployee> patchDoc)
    {
        var employee = await _employeeService.GetAsync(id);
        var updateEmployee = new UpdateEmployee
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            BirthDate = employee.BirthDate,
            BossId = employee.BossId,
            CurrentSalary = employee.CurrentSalary,
            EmploymentDate = employee.EmploymentDate,
            HomeAddress = employee.HomeAddress,
            Role = employee.Role
        };

        patchDoc.ApplyTo(updateEmployee, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _employeeService.UpdateAsync(id, updateEmployee);

        return NoContent();
    }

    /// <summary>
    ///     Deletes an employee
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _employeeService.DeleteAsync(id);

        return NoContent();
    }
}