using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Entities;
using TestTaskUkrPoshta.Services.Interfaces;
using TestTaskUkrPoshta.ViewModels;

namespace TestTaskUkrPoshta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISqlManager _sqlManager;

        public HomeController(ISqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }

        public async Task<IActionResult> Index()
        {
            var company = await _sqlManager.GetCompany();

            var model = new MainViewModel
            {
                Company = company
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var model = await CreateEmployeeModel(new EmployeeFilter());

            return View("Employees", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeesAsync(EmployeeFilter filter)
        {
            var model = await CreateEmployeeModel(filter);

            return View("Employees", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeForSalary()
        {
            var model = await CreateEmployeeModel(new EmployeeFilter());

            return View("SalaryReport", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetSalaryReportAsync(EmployeeFilter filter)
        {
            var report = await _sqlManager.GetSalaryReport(filter);

            return File(report, "text/plain", "SalaryReport.txt");
        }

        [HttpGet("/GetEmployee/{id:int}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _sqlManager.GetEmployee(id);
            var departments = await _sqlManager.GetDepartments();
            var positions = await _sqlManager.GetPositions();

            var model = new EmployeeCardViewModel
            {
                Employee = employee,
                Departments = departments.Select(s => new SelectListItem(s.Title, s.Id.ToString(), s.Title == employee.Department)),
                Positions = positions.Select(s => new SelectListItem(s.Title, s.Id.ToString(), s.Title == employee.Position))
            };

            return View("EmployeeCard", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(EmployeeFullInfo employee)
        {
            await _sqlManager.UpdateEmployee(employee);

            return RedirectToAction("GetEmployees");
        }

        private async Task<EmployeesViewModel> CreateEmployeeModel(EmployeeFilter filter)
        {
            var departments = await _sqlManager.GetDepartments();
            var positions = await _sqlManager.GetPositions();

            return new EmployeesViewModel()
            {
                Employees = await _sqlManager.GetEmployees(filter),
                Departments = AddNoneItem(departments.Select(s => new SelectListItem(s.Title, s.Id.ToString(), s.Id == filter.DepartmentId))),
                Positions = AddNoneItem(positions.Select(s => new SelectListItem(s.Title, s.Id.ToString(), s.Id == filter.PositionId))),
                Filter = filter
            };
        }

        private IEnumerable<SelectListItem> AddNoneItem(IEnumerable<SelectListItem> oldSelectList)
        {
            var newSelectList = new List<SelectListItem> { new("None", "") };
            newSelectList.AddRange(oldSelectList);

            return newSelectList;
        }
    }
}
