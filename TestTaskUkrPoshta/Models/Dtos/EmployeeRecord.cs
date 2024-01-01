namespace TestTaskUkrPoshta.Models.Dtos
{
    public record EmployeeRecord
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string Department { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public string Position { get; set; } = string.Empty;
        public int Salary { get; set; }
        public DateTime DateOfHire { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
