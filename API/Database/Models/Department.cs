using API.Database.Base;

namespace API.Database.Models;

public class Department  : BaseEntity, IAuditable
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Employee> Employees  { get; set; }
}
