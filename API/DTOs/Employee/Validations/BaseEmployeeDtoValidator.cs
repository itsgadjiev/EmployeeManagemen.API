using API.Database;
using API.Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs.Employee.Validations;

public class BaseEmployeeDtoValidator : AbstractValidator<BaseEmployeeDto>
{
    private readonly AppDbContext _appDbContext;

    public BaseEmployeeDtoValidator(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        RuleFor(x => x.Name)
            .NotNull();

        RuleFor(x => x.Name)
            .MinimumLength(2)
            .MaximumLength(40)
            .NotEmpty();

        RuleFor(x => x.Surname)
            .MinimumLength(2)
            .MaximumLength(40)
            .NotEmpty();

        RuleFor(x => x.Age)
           .NotNull();

        RuleFor(x => (int)x.Age)
            .LessThan(200)
            .GreaterThan(15);

        RuleFor(x => x.DepartmentId)
            .NotNull()
            .MustAsync(IsExsistingDepartment)
            .WithMessage("Department is not exsisting");

        RuleFor(x => x.Image)
            .NotNull()
            .Must(IsValidImage)
            .WithMessage("Not valid image");

    }

    public async Task<bool> IsExsistingDepartment(int id, CancellationToken arg)
    {
        return await _appDbContext.Departments.FirstOrDefaultAsync(x => x.Id == id) != null;
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
        {
            return false;
        }
        if (file.Length > 2097152)
        {
            return false;
        }
        return true;
    }



}
