using Microsoft.AspNetCore.Mvc;

namespace TestTaskUkrPoshta.Models.Entities
{
    public class EmployeeFullInfo
    {
        [FromForm]
        public int Id { get; set; }
        [FromForm]
        public string FullName { get; set; } = string.Empty;
        [FromForm]
        public int DepartmentId { get; set; }
        [FromForm]
        public int PositionId { get; set; }
        [FromForm]
        public int Salary { get; set; }
        [FromForm]
        public DateTime DateOfBirth { get; set; }
        [FromForm]
        public DateTime DateOfHire { get; set; }
        [FromForm]
        public string Address { get; set; } = string.Empty;
        [FromForm]
        public string Phone { get; set; } = string.Empty;
    }
}