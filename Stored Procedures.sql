CREATE TYPE CompanyType AS TABLE(
  Title NVARCHAR(255) NOT NULL,
  Description NVARCHAR(max) NULL
)
GO

CREATE OR ALTER PROCEDURE InsertCompanies
  @companies CompanyType READONLY
AS
BEGIN
  DECLARE @count INT
  SET @count = (
    SELECT COUNT(*)
    FROM @companies
    )

  INSERT INTO Test.dbo.Company(Title, Description)
  SELECT Title, Description
  FROM @companies

  SELECT TOP(@count) *
  FROM Test.dbo.Company
  ORDER BY Id DESC
END
GO

CREATE TYPE DepartmentType AS TABLE(
  Title NVARCHAR(255) NOT NULL,
  CompanyId INT NOT NULL
)
GO

CREATE OR ALTER PROCEDURE InsertDepartments
  @departments DepartmentType READONLY
AS
BEGIN
  DECLARE @count INT
  SET @count = (
    SELECT COUNT(*)
    FROM @departments
    )

  INSERT INTO Test.dbo.Department(Title, CompanyId)
  SELECT Title, CompanyId
  FROM @departments

  SELECT TOP(@count) *
  FROM Test.dbo.Department
  ORDER BY Id DESC
END
GO

CREATE TYPE PositionType AS TABLE(
  Title NVARCHAR(255) NOT NULL
)
GO

CREATE OR ALTER PROCEDURE InsertPositions
  @positions PositionType READONLY
AS
BEGIN
  DECLARE @count INT
  SET @count = (
    SELECT COUNT(*)
    FROM @positions
    )

  INSERT INTO Test.dbo.Position(Title)
  SELECT Title
  FROM @positions

  SELECT TOP(@count) *
  FROM Test.dbo.Position
  ORDER BY Id DESC
END
GO

CREATE TYPE DepartmentPositionType AS TABLE(
  DepartmentId INT NOT NULL,
  PositionId INT NOT NULL
)
GO

CREATE OR ALTER PROCEDURE InsertDepartmentPositions
  @departmentPositions DepartmentPositionType READONLY
AS
BEGIN
  DECLARE @count INT
  SET @count = (
    SELECT COUNT(*)
    FROM @departmentPositions
    )

  INSERT INTO Test.dbo.DepartmentPosition(DepartmentId, PositionId)
  SELECT DepartmentId, PositionId
  FROM @departmentPositions

  SELECT TOP(@count) *
  FROM Test.dbo.DepartmentPosition
  ORDER BY Id DESC
END
GO

CREATE OR ALTER PROCEDURE InsertEmployeeInfo
	@FullName NVARCHAR(255),
    @DepartmentId INT,
    @PositionId INT,
    @Salary INT,
    @DateOfBirth DATE,
    @DateOfHire DATE,
    @Address NVARCHAR(255),
    @Phone NVARCHAR(255)
AS
BEGIN
	DECLARE @infoId INT
	INSERT INTO Test.dbo.EmployeePrivateInfo(FullName, Address, DateOfBirth, Phone)
	VALUES(@FullName, @Address, @DateOfBirth, @Phone)
	SET @infoId = SCOPE_IDENTITY()

	DECLARE @departmentPositionId INT
	SELECT TOP(1) @departmentPositionId = Id
	FROM Test.dbo.DepartmentPosition
	WHERE (	
      	DepartmentId = (
            		SELECT TOP(1) Id
            		FROM Test.dbo.Department
            		WHERE Id = @DepartmentId)
      	AND
      	PositionId = (
            		SELECT TOP(1) Id
            		FROM Test.dbo.Position
            		WHERE Id = @PositionId)
		)

	INSERT INTO Test.dbo.Employee(DateOfHire, Salary, DepartmentPositionId, EmployeePrivateInfoId)
	VALUES(@DateOfHire, @Salary, @departmentPositionId, @infoId)
END
GO

CREATE OR ALTER PROCEDURE InsertEmployeeInfo
	@Id INT,
	@FullName NVARCHAR(255),
    @DepartmentId INT,
    @PositionId INT,
    @Salary INT,
    @DateOfBirth DATE,
    @DateOfHire DATE,
    @Address NVARCHAR(255),
    @Phone NVARCHAR(255)
AS
BEGIN
	DECLARE @departmentPositionId INT
	SELECT TOP(1) @departmentPositionId = Id
	FROM Test.dbo.DepartmentPosition
	WHERE (	
	   	DepartmentId = (
      		SELECT TOP(1) Id
      		FROM Test.dbo.Department
      		WHERE Id = @DepartmentId)
   		AND
   		PositionId = (
      		SELECT TOP(1) Id
      		FROM Test.dbo.Position
      		WHERE Id = @PositionId)
	)

	UPDATE Test.dbo.Employee
	SET DateOfHire = @DateOfHire, Salary = @Salary, DepartmentPositionId = @departmentPositionId
	WHERE Id = @Id

	UPDATE Test.dbo.EmployeePrivateInfo
	SET FullName = @FullName, Address = @Address, DateOfBirth = @DateOfBirth, Phone = @Phone
	WHERE Id = (
    SELECT TOP(1) EmployeePrivateInfoId
    FROM Test.dbo.Employee
    WHERE Id = @Id
)
END
GO

CREATE OR ALTER PROCEDURE GetEmployees
	@DepartmentId INT NULL,
	@PositionId INT NULL,
	@SalaryFrom INT NULL,
	@SalaryTo INT NULL,
	@Search NVARCHAR(max) NULL,
	@DateOfBirthFrom DATE NULL,
	@DateOfBirthTo DATE NULL,
	@DateOfHireFrom DATE NULL,
	@DateOfHireTo DATE NULL
AS
BEGIN
	SELECT *
	FROM master.dbo.EmployeeInfo
	WHERE (LOWER(FullName) LIKE '%' + @Search + '%' OR LOWER(Address) LIKE '%' + @Search + '%' OR LOWER(Phone) LIKE '%' + @Search + '%')
	AND (@DepartmentId IS NULL OR DepartmentId = @DepartmentId)
	AND (@PositionId IS NULL OR PositionId = @PositionId)
	AND (@SalaryFrom IS NULL OR Salary >= @SalaryFrom)
	AND (@SalaryTo IS NULL OR Salary <= @SalaryTo)
	AND (@DateOfBirthFrom IS NULL OR DateOfBirth >= @DateOfBirthFrom)
	AND (@DateOfBirthTo IS NULL OR DateOfBirth <= @DateOfBirthTo)
	AND (@DateOfHireFrom IS NULL OR DateOfHire >= @DateOfHireFrom)
	AND (@DateOfHireTo IS NULL OR DateOfHire <= @DateOfHireTo)
END
GO

CREATE OR ALTER PROCEDURE GetCompany
AS
BEGIN
	SELECT TOP(1) *
	FROM Test.dbo.Company
END
GO

CREATE OR ALTER PROCEDURE GetDepartments
AS
BEGIN
	SELECT *
	FROM Test.dbo.Department
END
GO

CREATE OR ALTER PROCEDURE GetPositions
AS
BEGIN
	SELECT *
	FROM Test.dbo.Position
END
GO

CREATE OR ALTER PROCEDURE GetEmployee
	@Id INT
AS
BEGIN
	SELECT *
	FROM master.dbo.EmployeeInfo
	WHERE Id = @id
END
GO