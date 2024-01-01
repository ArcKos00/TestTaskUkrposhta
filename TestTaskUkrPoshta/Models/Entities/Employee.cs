namespace TestTaskUkrPoshta.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public int Salary { get; set; }
        public DateTime DateOfHire { get; set; }
        public int DepartmentPositionId { get; set; }
        public int EmployeePrivateInfo { get; set; }
    }
}
