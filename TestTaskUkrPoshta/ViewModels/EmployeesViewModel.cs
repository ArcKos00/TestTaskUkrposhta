using Microsoft.AspNetCore.Mvc.Rendering;
using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Dtos;

namespace TestTaskUkrPoshta.ViewModels
{
    public class EmployeesViewModel
    {
        public IEnumerable<EmployeeRecord> Employees { get; set; } = Enumerable.Empty<EmployeeRecord>();
        public IEnumerable<SelectListItem> Departments { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Positions { get; set; } = Enumerable.Empty<SelectListItem>();
        public EmployeeFilter Filter { get; set; } = new EmployeeFilter();
    }
}
