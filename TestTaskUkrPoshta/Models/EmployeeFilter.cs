using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace TestTaskUkrPoshta.Models
{
    public record EmployeeFilter
    {
        [FromForm]
        public int? DepartmentId { get; set; }
        [FromForm]
        public int? PositionId { get; set; }
        [FromForm]
        public int? SalaryFrom { get; set; }
        [FromForm]
        public int? SalaryTo { get; set; }
        [FromForm]
        public string? Search { get; set; }
        [FromForm]
        public DateTime? DateOfBirthFrom { get; set; }
        [FromForm]
        public DateTime? DateOfBirthTo { get; set; }
        [FromForm]
        public DateTime? DateOfHireFrom { get; set; }
        [FromForm]
        public DateTime? DateOfHireTo { get; set; }

        public Dictionary<string, object> GetFilledProperties()
        {
            var propertyInfo = GetType().GetProperties().AsEnumerable();

            var preparationList = new List<KeyValuePair<string, object>>();

            foreach (var property in propertyInfo.Where(c => c.GetValue(this) != null))
            {
                    preparationList.Add(new(property.Name, property.GetValue(this)!));
            }

            return new(preparationList);
        }
    }
}
