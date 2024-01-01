namespace TestTaskUkrPoshta.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }
}
