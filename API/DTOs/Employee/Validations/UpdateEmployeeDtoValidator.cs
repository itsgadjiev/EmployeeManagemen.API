using API.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs.Employee.Validations;

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    private readonly AppDbContext _appDbContext;

    public UpdateEmployeeDtoValidator(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        //RuleFor(x => x.EmployeeId)
        //    .MustAsync(IsExsistingEmployee)
        //    .WithMessage("{PropertyName} must be present");

        Include(new BaseEmployeeDtoValidator(_appDbContext));
    }

    public async Task<bool> IsExsistingEmployee(int emplyeeId, CancellationToken arg)
    {
        return await _appDbContext.Employees.FirstOrDefaultAsync(x => x.Id == emplyeeId) != null;
    }
}
