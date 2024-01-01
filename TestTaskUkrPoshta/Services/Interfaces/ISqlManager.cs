using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Dtos;
using TestTaskUkrPoshta.Models.Entities;

namespace TestTaskUkrPoshta.Services.Interfaces
{
    public interface ISqlManager
    {
        Task InitializeData();
        Task CreateDataBase();
        Task CreateTables();
        Task SeedData();
        Task<bool> InsertEmployee(EmployeeFullInfo employeeInfo);
        Task<EmployeeRecord> GetEmployee(int id);
        Task<bool> UpdateEmployee(EmployeeFullInfo employeeInfo);
        Task<IEnumerable<EmployeeRecord>> GetEmployees(EmployeeFilter filter);
        Task<MemoryStream> GetSalaryReport(EmployeeFilter filter);
        Task<Company> GetCompany();
        Task<IEnumerable<Department>> GetDepartments();
        Task<IEnumerable<Position>> GetPositions();
    }
}
