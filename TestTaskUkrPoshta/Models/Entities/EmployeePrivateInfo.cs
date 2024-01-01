namespace TestTaskUkrPoshta.Models.Entities
{
    public class EmployeePrivateInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
