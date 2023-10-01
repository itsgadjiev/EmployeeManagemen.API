namespace API.DTOs.Employee
{
    public class BaseEmployeeDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public byte Age { get; set; }
        public double Salary { get; set; }
        public IFormFile Image { get; set; }
        public int DepartmentId { get; set; }
    }
}

