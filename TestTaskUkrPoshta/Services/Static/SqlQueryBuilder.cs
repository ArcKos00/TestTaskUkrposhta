using System.Reflection;
using System.Text;
using TestTaskUkrPoshta.Models;
using TestTaskUkrPoshta.Models.Entities;

namespace TestTaskUkrPoshta.StaticServices
{
    public static class SqlQueryBuilder
    {
        private const string _database = "[Test].[dbo]";
        private static readonly StringBuilder Builder = new();

        public static IEnumerable<string> GetCreateDatabase()
            => new List<string>
            {
                """
                USE [master]
                
                CREATE DATABASE [Test]
                 CONTAINMENT = NONE
                 ON  PRIMARY 
                ( NAME = N'Test', FILENAME = N'/var/opt/mssql/data/Test.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
                 LOG ON 
                ( NAME = N'Test_log', FILENAME = N'/var/opt/mssql/data/Test_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
                 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
                
                IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
                begin
                EXEC [Test].[dbo].[sp_fulltext_database] @action = 'enable'
                end
                
                ALTER DATABASE [Test] SET ANSI_NULL_DEFAULT OFF 
                
                ALTER DATABASE [Test] SET ANSI_NULLS OFF 
                
                ALTER DATABASE [Test] SET ANSI_PADDING OFF 
                
                ALTER DATABASE [Test] SET ANSI_WARNINGS OFF 
                
                ALTER DATABASE [Test] SET ARITHABORT OFF 
                
                ALTER DATABASE [Test] SET AUTO_CLOSE OFF 
                
                ALTER DATABASE [Test] SET AUTO_SHRINK OFF 
                
                ALTER DATABASE [Test] SET AUTO_UPDATE_STATISTICS ON 
                
                ALTER DATABASE [Test] SET CURSOR_CLOSE_ON_COMMIT OFF 
                
                ALTER DATABASE [Test] SET CURSOR_DEFAULT  GLOBAL 
                
                ALTER DATABASE [Test] SET CONCAT_NULL_YIELDS_NULL OFF 
                
                ALTER DATABASE [Test] SET NUMERIC_ROUNDABORT OFF 
                
                ALTER DATABASE [Test] SET QUOTED_IDENTIFIER OFF 
                
                ALTER DATABASE [Test] SET RECURSIVE_TRIGGERS OFF 
                
                ALTER DATABASE [Test] SET  DISABLE_BROKER 
                
                ALTER DATABASE [Test] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
                
                ALTER DATABASE [Test] SET DATE_CORRELATION_OPTIMIZATION OFF 
                
                ALTER DATABASE [Test] SET TRUSTWORTHY OFF 
                
                ALTER DATABASE [Test] SET ALLOW_SNAPSHOT_ISOLATION OFF 
                
                ALTER DATABASE [Test] SET PARAMETERIZATION SIMPLE 
                
                ALTER DATABASE [Test] SET READ_COMMITTED_SNAPSHOT OFF 
                
                ALTER DATABASE [Test] SET HONOR_BROKER_PRIORITY OFF 
                
                ALTER DATABASE [Test] SET RECOVERY FULL 
                
                ALTER DATABASE [Test] SET  MULTI_USER 
                
                ALTER DATABASE [Test] SET PAGE_VERIFY CHECKSUM  
                
                ALTER DATABASE [Test] SET DB_CHAINING OFF 
                
                ALTER DATABASE [Test] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
                
                ALTER DATABASE [Test] SET TARGET_RECOVERY_TIME = 60 SECONDS 
                
                ALTER DATABASE [Test] SET DELAYED_DURABILITY = DISABLED 
                
                ALTER DATABASE [Test] SET ACCELERATED_DATABASE_RECOVERY = OFF  
                
                ALTER DATABASE [Test] SET QUERY_STORE = ON
                
                ALTER DATABASE [Test] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
                
                ALTER DATABASE [Test] SET  READ_WRITE
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