using Microsoft.AspNetCore.Mvc.Rendering;
using TestTaskUkrPoshta.Models.Dtos;
using TestTaskUkrPoshta.Models.Entities;

namespace TestTaskUkrPoshta.ViewModels
{
    public class EmployeeCardViewModel
    {
        public EmployeeRecord Employee { get; set; } = new();
        public EmployeeFullInfo EmployeeFullInfo { get; set; } = new();
        public IEnumerable<SelectListItem> Departments { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Positions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
