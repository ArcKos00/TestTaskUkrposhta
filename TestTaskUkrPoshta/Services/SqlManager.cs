using System.Text;
using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Dtos;
using TestTaskUkrPoshta.Models.Entities;
using TestTaskUkrPoshta.Repositories.Interfaces;
using TestTaskUkrPoshta.Services.Interfaces;
using TestTaskUkrPoshta.StaticServices;

namespace TestTaskUkrPoshta.Services
{
    public class SqlManager(ISqlRepository _repository) : ISqlManager
    {
        public async Task InitializeData()
        {
            await CreateTables();
            await SeedData();
        }

        public async Task CreateDataBase()
        {
            var createDB = SqlQueryBuilder.GetCreateDatabase();
            await _repository.ExecuteSqlRawAsync(createDB);
        }

        public async Task CreateTables()
        {
            var query = SqlQueryBuilder.GetCreateTablesQuery();
            await _repository.ExecuteSqlRawAsync(query);
        }

        public async Task SeedData()
        {
            var random = new Random();
            var employeeCounter = 1;

            var createCompany = SqlQueryBuilder.GetCompanyInsertQuery(new List<Company>
            {
                new()
                {
                    Title = "Ukrposhta",
                    Description =
                        "Ukrposhta is a state-owned postal service company subordinated to the Ministry of Infrastructure of Ukraine, the national postal operator."
                }
            });

            foreach (var company in await _repository.ExecuteSqlRawAsync<Company>(createCompany))
            {
                var createDepartment = SqlQueryBuilder.GetDepartmentInsertQuery(new List<Department>
                {
                    new()
                    {
                        CompanyId = company.Id,
                        Title = "IT"
                    },
                    new()
                    {
                        CompanyId = company.Id,
                        Title = "Sience"
                    },
                    new()
                    {
                        CompanyId = company.Id,
                        Title = "Delivery"
                    }
                });

                var departments = await _repository.ExecuteSqlRawAsync<Department>(createDepartment);

                var createPositions = SqlQueryBuilder.GetPositionInsertQuery(new List<Position>
                {
                    new()
                    {
                        Title = "Senior Employee"
                    },
                    new()
                    {
                        Title = "Junior Employee"
                    },
                    new()
                    {
                        Title = "Middle Employee"
                    }
                });
                var positions = await _repository.ExecuteSqlRawAsync<Position>(createPositions);

                var createDepartmentPosition = SqlQueryBuilder.GetDepartmentPositionInsertQuery(ElementSelection(departments, positions));
                foreach (var departmentPosition in await _repository.ExecuteSqlRawAsync<DepartmentPosition>(createDepartmentPosition))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var createEmployee = SqlQueryBuilder.GetEmployeeInsertQuery(new()
                        {
                            FullName = $"Sergey Semenovich Petrenko {employeeCounter}",
                            Address = $"Stepan Bandera Avenue {employeeCounter}",
                            DepartmentId = departmentPosition.DepartmentId,
                            PositionId = departmentPosition.PositionId,
                            Salary = 1000 * employeeCounter / 2,
                            Phone = "+380597584586",
                            DateOfBirth = new DateTime(random.NextInt64(DateTime.UnixEpoch.Ticks, DateTime.UtcNow.AddYears(-1).Ticks)),
                            DateOfHire = new DateTime(random.NextInt64(DateTime.UtcNow.AddYears(-1).Ticks, DateTime.UtcNow.Ticks)),
                        });

                        await _repository.ExecuteSqlRawAsync(createEmployee);
                        employeeCounter++;
                    }
                }
            }
        }

        private List<DepartmentPosition> ElementSelection(IEnumerable<Department> departments, IEnumerable<Position> positions)
        {
            var result = new List<DepartmentPosition>();
            foreach (var department in departments)
            {
                foreach (var position in positions)
                {
                    result.Add(new()
                    {
                        DepartmentId = department.Id,
                        PositionId = position.Id
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<EmployeeRecord>> GetEmployees(EmployeeFilter filter)
        {
            var query = SqlQueryBuilder.GetEmployeesQuery(filter);

            return await _repository.ExecuteSqlRawAsync<EmployeeRecord>(query);
        }

        public async Task<MemoryStream> GetSalaryReport(EmployeeFilter filter)
        {
            var query = SqlQueryBuilder.GetEmployeesQuery(filter);

            var result = await _repository.ExecuteSqlRawAsync<EmployeeRecord>(query);

            var report = ReportCreator.CreateReport(result);

            return new MemoryStream(Encoding.UTF8.GetBytes(report));
        }

        public async Task<Company> GetCompany()
        {
            var getCompanyQuery = SqlQueryBuilder.GetCompanyQuery();

            return (await _repository.ExecuteSqlRawAsync<Company>(getCompanyQuery)).FirstOrDefault() ?? new Company();
        }

        public async Task<IEnumerable<Department>> GetDepartments()
        {
            var getDepartmentsQuery = SqlQueryBuilder.GetDepartmentQuery();

            return await _repository.ExecuteSqlRawAsync<Department>(getDepartmentsQuery);
        }

        public async Task<IEnumerable<Position>> GetPositions()
        {
            var getPositionQuery = SqlQueryBuilder.GetPositionQuery();

            return await _repository.ExecuteSqlRawAsync<Position>(getPositionQuery);
        }

        public async Task<EmployeeRecord> GetEmployee(int id)
        {
            var getEmployeeQuery = SqlQueryBuilder.GetEmployee(id);

            return (await _repository.ExecuteSqlRawAsync<EmployeeRecord>(getEmployeeQuery)).FirstOrDefault() ?? new EmployeeRecord();
        }

        public async Task<bool> UpdateEmployee(EmployeeFullInfo employeeInfo)
        {
            try
            {
                var updateEmployee = SqlQueryBuilder.GetEmployeeUpdateQuery(employeeInfo);
                await _repository.ExecuteSqlRawAsync(updateEmployee);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertEmployee(EmployeeFullInfo employeeInfo)
        {
            try
            {
                var createEmployee = SqlQueryBuilder.GetEmployeeInsertQuery(employeeInfo);
                await _repository.ExecuteSqlRawAsync(createEmployee);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
