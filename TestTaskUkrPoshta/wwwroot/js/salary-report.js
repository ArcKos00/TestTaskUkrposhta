var employees = [];

function loadData(incomeEmployees) {
    employees = incomeEmployees;

    drawElements(employees);
}

function createTableRow(element) {
    return `<tr>
                    <td>${element.fullName}</td>
                    <td>${element.department}</td>
                    <td>${element.position}</td>
                    <td>${new Date(element.dateOfHire).toLocaleDateString()}</td>
                    <td>${element.salary} $</td>
                    <td>${element.address}</td>
                    <td>${element.phone}</td>
                    <td>${new Date(element.dateOfBirth).toLocaleDateString()}</td>
                </tr>`
}

function drawElements(elements) {

    const createdElements = [];
    for (const element of elements) {
        createdElements.push(createTableRow(element))
    }

    document.querySelector('#table').innerHTML = createdElements.join('\n')
}

function applyFilters() {
    const filteredElements = departmentFilter(employees);
    drawElements(filteredElements);
}

function departmentFilter(elements) {
    const value = document.querySelector('#DepartmentId').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => element.departmentId == value);
    }

    return positionFilter(filtered);
}

function positionFilter(elements) {
    const value = document.querySelector('#PositionId').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => element.positionId == value);
    }

    return hireFromFilter(filtered);
}

function hireFromFilter(elements) {
    const value = document.querySelector('#HireFrom').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => new Date(element.dateOfHire) >= new Date(value));
    }

    return HireToFilter(filtered);
}

function HireToFilter(elements) {
    const value = document.querySelector('#HireTo').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => new Date(element.dateOfHire) <= new Date(value));
    }

    return birthFromFilter(filtered);
}

function birthFromFilter(elements) {
    const value = document.querySelector('#BirthFrom').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => new Date(element.dateOfBirth) >= new Date(value));
    }

    return birthToFilter(filtered);
}

function birthToFilter(elements) {
    const value = document.querySelector('#BirthTo').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => new Date(element.dateOfBirth) <= new Date(value));
    }

    return salaryFromFilter(filtered);
}

function salaryFromFilter(elements) {
    const value = document.querySelector('#SalaryFrom').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => element.salary >= value);
    }

    return salaryToFilter(filtered);
}

function salaryToFilter(elements) {
    const value = document.querySelector('#SalaryTo').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => element.salary <= value);
    }

    return searchFilter(filtered);
}

function searchFilter(elements) {
    const value = document.querySelector('#Search').value;
    var filtered = []

    if (!!!value) {
        filtered = elements;
    }
    else {
        filtered = elements.filter(element => element.fullName.includes(value) || element.address.includes(value) || element.phone.includes(value));
    }

    return filtered;
}