using System.Reflection;
using System.Text;
using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Entities;

namespace TestTaskUkrPoshta.StaticServices
{
    public static class SqlQueryBuilder
    {
        private const string _databaseName = "Task";
        private const string _database = $"[{_databaseName}].[dbo]";
        private static readonly StringBuilder Builder = new();

        public static IEnumerable<string> GetCreateDatabase()
            => new List<string>
            {
                $"""
                 USE [master]
                 
                 CREATE DATABASE [{_databaseName}]
                  CONTAINMENT = NONE
                  ON  PRIMARY 
                 ( NAME = N'{_databaseName}', FILENAME = N'/var/opt/mssql/data/{_databaseName}.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
                  LOG ON 
                 ( NAME = N'{_databaseName}_log', FILENAME = N'/var/opt/mssql/data/{_databaseName}_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
                  WITH CATALOG_COLLATION = DATABASE_DEFAULT
                 """,
            };

        public static IEnumerable<string> GetCreateTablesQuery()
            => new List<string>{$"""
                                CREATE TABLE {_database}.[Company] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [Title] NVARCHAR(255) NOT NULL,
                                  [Description] NVARCHAR(max) NULL
                                )
                                """,
                                $"""
                                CREATE TABLE {_database}.[Department] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [Title] NVARCHAR(255) NOT NULL,
                                  [CompanyId] INT NOT NULL ,
                                  FOREIGN KEY (CompanyId)REFERENCES Company(Id)
                                )
                                """,
                                $"""
                                CREATE TABLE {_database}.[Position] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [Title] NVARCHAR(255) UNIQUE NOT NULL,
                                )
                                """,
                                $"""
                                CREATE TABLE  {_database}.[DepartmentPosition] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [DepartmentId] INT NOT NULL,
                                  [PositionId] INT NOT NULL,
                                  FOREIGN KEY (DepartmentId) REFERENCES Department(Id),
                                  FOREIGN KEY (PositionId) REFERENCES Position(Id)
                                )
                                """,
                                $"""
                                CREATE TABLE {_database}.[EmployeePrivateInfo] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [FullName] NVARCHAR(255) NOT NULL,
                                  [Address] NVARCHAR(255) NOT NULL,
                                  [DateOfBirth] DATE NOT NULL,
                                  [Phone] NVARCHAR(20) NOT NULL,
                                )
                                """,
                                $"""
                                CREATE TABLE {_database}.[Employee] (
                                  [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                  [DateOfHire] DATE NOT NULL,
                                  [Salary] INTEGER NOT NULL,
                                  [DepartmentPositionId] INT NOT NULL,
                                  [EmployeePrivateInfoId] INT NOT NULL,
                                  FOREIGN KEY (DepartmentPositionId) REFERENCES DepartmentPosition(Id),
                                  FOREIGN KEY (EmployeePrivateInfoId) REFERENCES EmployeePrivateInfo(Id)
                                )
                                """,
                                $"""
                                CREATE OR ALTER VIEW EmployeeInfo AS
                                SELECT
                                  empl.Id,
                                  emplInfo.FullName,
                                  dep.Id As DepartmentId,
                                  dep.Title AS Department,
                                  pos.Id AS PositionId,
                                  pos.Title AS Position,
                                  empl.Salary,
                                  empl.DateOfHire,
                                  emplInfo.Address,
                                  emplInfo.DateOfBirth,
                                  emplInfo.Phone
                                FROM {_database}.Employee empl
                                LEFT JOIN {_database}.EmployeePrivateInfo emplInfo ON empl.EmployeePrivateInfoId = emplInfo.Id
                                LEFT JOIN {_database}.DepartmentPosition depPos ON empl.DepartmentPositionId = depPos.Id
                                LEFT JOIN {_database}.Department dep ON depPos.DepartmentId = dep.Id
                                LEFT JOIN {_database}.Position pos ON depPos.PositionId = pos.Id
                                """};

        public static string GetCompanyInsertQuery(IEnumerable<Company> entities)
        {
            Builder.Append($"INSERT INTO {_database}.Company(Title, Description){Environment.NewLine}VALUES");
            foreach (var entity in entities)
            {
                Builder.Append($" (\'{entity.Title}\', \'{entity.Description}\'),");
            }

            Builder
                .Remove(Builder.Length - 1, 1)
                .Append(GetReturnBlock(entities.Count(), nameof(Company)));

            return HandleQueryBuilder(Builder);
        }

        public static string GetDepartmentInsertQuery(IEnumerable<Department> entities)
        {
            Builder.Append($"INSERT INTO {_database}.Department(Title, CompanyId){Environment.NewLine}VALUES");
            foreach (var entity in entities)
            {
                Builder.Append($" (\'{entity.Title}\', \'{entity.CompanyId}\'),");
            }

            Builder
                .Remove(Builder.Length - 1, 1)
                .Append(GetReturnBlock(entities.Count(), nameof(Department)));

            return HandleQueryBuilder(Builder);
        }

        public static string GetPositionInsertQuery(IEnumerable<Position> entities)
        {
            Builder.Append($"INSERT INTO {_database}.Position(Title){Environment.NewLine}VALUES");
            foreach (var entity in entities)
            {
                Builder.Append($" (\'{entity.Title}\'),");
            }

            Builder
                .Remove(Builder.Length - 1, 1)
                .Append(GetReturnBlock(entities.Count(), nameof(Position)));

            return HandleQueryBuilder(Builder);
        }

        public static string GetDepartmentPositionInsertQuery(IEnumerable<DepartmentPosition> entities)
        {
            Builder.Append($"INSERT INTO {_database}.DepartmentPosition(DepartmentId, PositionId){Environment.NewLine}VALUES");
            foreach (var entity in entities)
            {
                Builder.Append($" (\'{entity.DepartmentId}\', '{entity.PositionId}'),");
            }

            Builder
                .Remove(Builder.Length - 1, 1)
                .Append(GetReturnBlock(entities.Count(), nameof(DepartmentPosition)));

            return HandleQueryBuilder(Builder);
        }

        public static string GetEmployeeInsertQuery(EmployeeFullInfo employee)
            => $"""
                 DECLARE @infoId INT
                 INSERT INTO {_database}.EmployeePrivateInfo(FullName, Address, DateOfBirth, Phone)
                 VALUES('{employee.FullName}', '{employee.Address}', {Formatter(employee.DateOfBirth)}, '{employee.Phone}')
                 SET @infoId = SCOPE_IDENTITY()
                 
                 DECLARE @departmentPositionId INT
                 SELECT TOP(1) @departmentPositionId = Id
                 FROM {_database}.DepartmentPosition
                 WHERE (	
                    	DepartmentId = (
                       		SELECT TOP(1) Id
                       		FROM {_database}.Department
                       		WHERE Id = {employee.DepartmentId})
                    	AND
                    	PositionId = (
                       		SELECT TOP(1) Id
                       		FROM {_database}.Position
                       		WHERE Id = {employee.PositionId})
                 )
                 
                 INSERT INTO {_database}.Employee(DateOfHire, Salary, DepartmentPositionId, EmployeePrivateInfoId)
                 VALUES({Formatter(employee.DateOfHire)}, {employee.Salary}, @departmentPositionId, @infoId)
                 """;

        public static string GetEmployeeUpdateQuery(EmployeeFullInfo employee)
            => $"""
                DECLARE @departmentPositionId INT
                SELECT TOP(1) @departmentPositionId = Id
                FROM {_database}.DepartmentPosition
                WHERE (	
                	DepartmentId = (
                		SELECT TOP(1) Id
                		FROM {_database}.Department
                		WHERE Id = {employee.DepartmentId})
                	AND
                	PositionId = (
                		SELECT TOP(1) Id
                		FROM {_database}.Position
                		WHERE Id = {employee.PositionId})
                )
                
                UPDATE {_database}.Employee
                SET DateOfHire = {Formatter(employee.DateOfHire)}, Salary = {employee.Salary}, DepartmentPositionId = @departmentPositionId
                WHERE Id = {employee.Id}

                UPDATE {_database}.EmployeePrivateInfo
                SET FullName = '{employee.FullName}', Address = '{employee.Address}', DateOfBirth = {Formatter(employee.DateOfBirth)}, Phone = '{employee.Phone}'
                WHERE Id = (
                    SELECT TOP(1) EmployeePrivateInfoId
                    FROM {_database}.Employee
                    WHERE Id = {employee.Id}
                )
                """;

        public static string GetEmployeesQuery(EmployeeFilter filter)
        {
            var properties = filter.GetFilledProperties();
            return $"""
                    SELECT *
                    FROM EmployeeInfo
                    {(properties.Count > 0
                        ? GetWhereStatement(properties)
                        : string.Empty)}
                    """;
        }

        public static string GetCompanyQuery()
            => $"""
                SELECT TOP(1) *
                FROM {_database}.Company
                """;

        public static string GetDepartmentQuery()
            => $"""
                SELECT *
                FROM {_database}.Department
                """;

        public static string GetPositionQuery()
            => $"""
                SELECT *
                FROM {_database}.Position
                """;

        public static string GetEmployee(int id)
            => $"""
                SELECT TOP(1) *
                FROM EmployeeInfo
                WHERE Id = {id}
                """;

        private static string Formatter(object value) => value switch
        {
            string str => str.ToLower(),
            Guid guid => guid.ToString(),
            DateTime datetime => $"\'{datetime:yyyy-MM-dd}\'",
            int integer => integer.ToString(),
            _ => string.Empty,
        };

        private static string GetWhereStatement(Dictionary<string, object> properties)
        {
            var sb = new StringBuilder();

            var idStr = "id";
            var toStr = "to";
            var fromStr = "from";
            var startsValue = "WHERE";

            foreach (var property in properties)
            {
                if (property.Value is string)
                {
                    sb.Append($"{startsValue} (LOWER(FullName) LIKE \'%{Formatter(property.Value)}%\' OR LOWER(Address) LIKE '%{Formatter(property.Value)}%' OR LOWER(Phone) LIKE '%{Formatter(property.Value)}%')");
                }
                else if (property.Key.EndsWith(fromStr, StringComparison.InvariantCultureIgnoreCase))
                {
                    sb.Append($"{startsValue} {GetPropertyName(property.Key, fromStr)} >= {Formatter(property.Value)}");
                }
                else if (property.Key.EndsWith(toStr, StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"{startsValue} {GetPropertyName(property.Key, toStr)} <= {Formatter(property.Value)}");
                }
                else if (property.Key.EndsWith(idStr, StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($"{startsValue} {property.Key} = {Formatter(property.Value)}");
                }

                startsValue = " AND";
            }

            return sb.ToString();
        }

        private static string GetPropertyName(string property, string select)
            => property[..^select.Length];

        private static string GetReturnBlock(int count, string tableName)
            => $"""

                SELECT TOP({count}) *
                FROM {_database}.{tableName}
                ORDER BY Id DESC
                """;

        private static string HandleQueryBuilder(StringBuilder builder)
        {
            var result = builder.ToString();
            builder.Clear();

            return result;
        }
    }
}