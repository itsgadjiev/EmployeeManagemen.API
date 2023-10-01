using API.Database.Base;

namespace API.Database.Models;

public class Employee : BaseEntity, IAuditable
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public byte Age { get; set; }
    public double Salary { get; set; }
    public string Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Department Department { get; set; }
    public int DepartmentId { get; set; }
}
