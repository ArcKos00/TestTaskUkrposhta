using System.Text;
using TestTaskUkrPoshta.Models.Dtos;

namespace TestTaskUkrPoshta.StaticServices
{
    public class ReportCreator
    {
        public static string CreateReport(IEnumerable<EmployeeRecord> employees)
        {
            var builder = new StringBuilder();

            var nameTitle = "Name";
            var departmentTitle = "Department";
            var positionTitle = "Position";
            var dateOfHireTitle = "Date of Hire";
            var salaryTitle = "Salary";

            var maxNameFieldLength = Math.Max(nameTitle.Length, employees.DefaultIfEmpty().MaxBy(m => m?.FullName.Length)?.FullName.Length ?? 0);
            var maxDepartmentFieldLength = Math.Max(departmentTitle.Length, employees.DefaultIfEmpty().MaxBy(m => m?.Department.Length)?.Department.Length ?? 0);
            var maxPositionFieldLength = Math.Max(positionTitle.Length, employees.DefaultIfEmpty().MaxBy(m => m?.Position.Length)?.Position.Length ?? 0);
            var maxSalaryFieldLength = Math.Max(dateOfHireTitle.Length, employees.DefaultIfEmpty().MaxBy(m => m?.Salary.ToString().Length)?.Salary.ToString().Length ?? 0);
            var maxDateOfHireLength = Math.Max(salaryTitle.Length, employees.FirstOrDefault()?.DateOfHire.ToString("yyyy-MM-dd").Length ?? 0);

            var header = $"{AlignString(nameTitle, maxNameFieldLength)} | {AlignString(departmentTitle, maxDepartmentFieldLength)} | {AlignString(positionTitle, maxPositionFieldLength)} | {AlignString(dateOfHireTitle, maxDateOfHireLength)} | {AlignString(salaryTitle, maxSalaryFieldLength)}";
            builder.Append(header + Environment.NewLine).Append(new string('_', header.Length) + Environment.NewLine);

            foreach (var employee in employees)
            {
                builder.Append($"{AlignString(employee.FullName, maxNameFieldLength)} | {AlignString(employee.Department, maxDepartmentFieldLength)} | {AlignString(employee.Position, maxPositionFieldLength)} | {AlignString(employee.DateOfHire.ToString("yyyy-MM-dd"), maxDateOfHireLength)} | {AlignString(employee.Salary.ToString(), maxSalaryFieldLength)}{Environment.NewLine}");
            }

            var sumTitle = "Sum";
            var sum = employees.Sum(s => s.Salary).ToString();

            builder.Append(Environment.NewLine + "Sum" + new string(' ', header.Length - (sumTitle.Length + sum.Length)) + sum);

            return builder.ToString();
        }

        private static string AlignString(string value, int commonLength)
            => value.Length < commonLength ? value + new string(' ', commonLength - value.Length) : value;
    }
}
