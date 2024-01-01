# TestTaskUkrposhta
1. код репозиторію

2. Існує таблиця з даними, в якій існують рядки, що повторюються, за значеннями деяких стовпців.
Необхідно отримати список рядків, що повторюються.
Потім видалити рядки, що повторюються, залишивши тільки по одному унікальному  рядку (залишити тільки найпізніші рядки).
По можливості навести кілька варіантів рішень.

DELETE
FROM table
  WHERE id IN (
    SELECT id
    FROM (
      SELECT id, MAX(created_at) AS created_at
      FROM table
      GROUP BY id, name
      ) AS t
  EXCEPT
    SELECT id, name
    FROM table
    ORDER BY created_at DESC

DELETE
FROM table
  WHERE id IN (
    SELECT id
    FROM (
      SELECT name, MAX(created_at) AS created_at
      FROM table
      GROUP BY name
      HAVING COUNT(*) > 1
        ) AS t
    )

3.![image](https://github.com/ArcKos00/TestTaskUkrposhta/assets/105163313/9a4127bc-c427-4147-b087-3b6a3107a39a)

- отримати всіх співробітників, незалежно від того, чи мають вони відповідності відділу
департаменті
- вивести середню зарплату по відділах

SELECT DEPARTMENT_ID, AVG(SALARY) AS AVG_SALARY
   FROM EMPLOYEES
   GROUP BY DEPARTMENT_ID


4.Опис таблиці EMPLOYEES

EMP_ID             NUMBER(4) NOT NULL
LAST_NAME   VARCHAR2(30) NOT NULL
FIRST_NAME  VARCHAR2(30)
DEPT_ID           NUMBER(2)
JOB_CAT          VARCHAR2(30)
SALARY            NUMBER
Потрібно вибрати ідентифікатор відділу, мінімальний розмір заробітної плати, а також максимальну зарплату, виплачену в цьому відділі, з урахуванням, що мінімальна заробітна плата становить менше 5000, і максимальна зарплата більша, ніж 15000.

SELECT DEPARTMENT_ID, MIN(SALARY) AS MIN_SALARY, MAX(SALARY) AS MAX_SALARY
FROM EMPLOYEES
WHERE SALARY < 5000 AND SALARY > 15000
GROUP BY DEPARTMENT_ID

5. ![image](https://github.com/ArcKos00/TestTaskUkrposhta/assets/105163313/a3512e6c-561b-482a-86d1-5e69ae661656)

5.1 - В таблиці EMPLOYEES, EMPLOYEE_ID є первинним ключем.
MGR_ID це ідентифікатор менеджерів і відноситься до EMPLOYEE_ID.
Dept_id є зовнішнім ключем до DEPARTMENT_ID колонки таблиці DEPARTMENTS.
В таблиці DEPARTMENTS department_id є первинним ключем.

Що станеться, якщо виконати та чому?
 DELETE
 FROM departments
 WHERE department id = 40;

буде помилка, оскільки деякі записи в таблиці EMPLOYEES мають посилання на даний запис

5.2 - В таблиці EMPLOYEES, EMPLOYEE_ID є первинним ключем.
MGR_ID це ідентифікатор менеджерів і відноситься до EMPLOYEE_ID.
Колонка JOB_ID – NOT NULL.

Що станеться, якщо виконати та чому?

DELETE employee_id, salary, job_id
FROM employees
            WHERE dept_id = 90;

буде помилка, оскільки неправильні аргументи для використання DELETE





