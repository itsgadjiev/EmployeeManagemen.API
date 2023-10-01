using API.Database;
using API.Database.Models;
using API.DTOs.Employee;
using API.DTOs.Employee.Validations;
using API.Services.abstracts;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public EmployeeController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _appDbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (user == null) { return NotFound(); }

            return Ok(user);
        }

        [HttpGet("get_all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery(Name = "name")] string? name,
            [FromQuery(Name = "surname")] string? surname,
            [FromQuery(Name = "min_salary")] double? minSalary,
            [FromQuery(Name = "max_salary")] double? maxSalary,
            [FromQuery(Name = "deparment_id")] int? departmentId,
            [FromQuery(Name = "sort")] string? sortField,
            [FromQuery(Name = "desc")] bool desc = false)
        {
            var query = _appDbContext.Employees.AsQueryable();

            query = (!string.IsNullOrEmpty(name) ? query.Where(e => e.Name == name) : query);
            query = (!string.IsNullOrEmpty(surname) ? query.Where(e => e.Surname == surname) : query);
            query = (minSalary != null ? query.Where(e => e.Salary >= minSalary) : query);
            query = (maxSalary != null ? query.Where(e => e.Salary <= maxSalary) : query);
            query = (departmentId != null ? query.Where(e => e.DepartmentId == departmentId) : query);

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "name":
                        query = desc ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name);
                        break;
                    case "surname":
                        query = desc ? query.OrderByDescending(e => e.Surname) : query.OrderBy(e => e.Surname);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }


            var list = await query.ToListAsync();

            return Ok(list);
        }


        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromForm] CreateEmployeeDto employeeDto)
        {
            var validations = await new CreateEmployeeDtoValidator(_appDbContext).ValidateAsync(employeeDto);
            if (validations.Errors.Any()) { return BadRequest(); }

            var employee = _mapper.Map<Employee>(employeeDto);
            employee.Image = employeeDto.Image.SaveFile(_webHostEnvironment.WebRootPath, "uploads/images");

            await _appDbContext.Employees.AddAsync(employee);
            await _appDbContext.SaveChangesAsync();
            //teacher bunu edende  System.InvalidOperationException: No route matches the supplied values. bele bir exception atirdi
            //bilmedim niye lakin ishleyirdi ozu ozune necese xarab oldu 
            //return CreatedAtAction(nameof(Get), new { id = employee.Id });
            return Created(nameof(Get), new { id = employee.Id });
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromForm] UpdateEmployeeDto employeeDto)
        {
            var validations = await new UpdateEmployeeDtoValidator(_appDbContext).ValidateAsync(employeeDto);
            if (validations.Errors.Any()) { return BadRequest(validations.Errors); }

            var employee = await _appDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null) { return NotFound(); }

            _mapper.Map(employeeDto, employee);

            if (employee.Image != null)
            {
                employeeDto.Image.RemoveFile(_webHostEnvironment.WebRootPath, "uploads/images", employee.Image);
                employee.Image = employeeDto.Image.SaveFile(_webHostEnvironment.WebRootPath, "uploads/images");
            }

            _appDbContext.Employees.Update(employee);

            await _appDbContext.SaveChangesAsync();
            return Ok(new { id = employee.Id, name = employee.Name });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _appDbContext.Employees.FindAsync(id);
            if (employee == null) { return NotFound(); }

            _appDbContext.Employees.Remove(employee);
            await _appDbContext.SaveChangesAsync();
            CustomFileService.RemoveFile(_webHostEnvironment.WebRootPath, "uploads/images", employee.Image);
            return NoContent();
        }
    }
}
