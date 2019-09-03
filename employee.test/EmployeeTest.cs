using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace employee.test
{
    public class EmployeeTest
    {
        
        private Employee employee;
        public EmployeeTest()
        {
            string[] employeeCSV =new string[]{ "emp6,emp3,1000",
                                        "emp4,emp3,1000",
                                        "emp1,,2000",      //CEO
                                        "emp2,emp1,1000",
                                        "emp3,emp1,1500",
                                        "emp7,emp6,1000",
                                        "emp8,emp3,1000"};

            employee=new Employee(employeeCSV);
        }

        [Fact]
        public void AllAmployeesLoaded()
        {
            IEnumerable<Employee> employees=employee.employees;
            
            Assert.NotNull(employees);
        }

        [Fact]
        public void CountemployeesWhoReportDirectToCEO()
        {
            IEnumerable<Employee> employees=employee.employees;
            int employeeCount= (from Employee employee in employees
                            select employee).Count();   
            Assert.Equal(2,employeeCount);
        }

        [Fact]
        public void SumAllEmployeesSalaries()
        {
           int salaries=employee.salaryBudget(employee); 
           Assert.Equal(8500,salaries);
        }
        
        /**
            There is no circular reference, i.e. a first employee
             reporting to a second employee that is also under
        the first employee. */
        [Fact]
        public void ValidateCircularReference()
        {
      
             string[] employeeCSV =new string[]{ "emp6,emp3,1000",
                                        "emp4,emp3,1000",
                                        "emp1,,2000",      //CEO
                                        "emp2,emp1,1000",
                                        "emp3,emp1,1500",
                                        "emp7,emp6,1000",
                                        "emp8,emp3,1000",
                                        "emp9,emp13,1000", //err
                                        "emp13,emp9,1000"}; //err


             Exception ex = Assert.Throws<Exception>(() => new Employee(employeeCSV));

            Assert.StartsWith("There is a circular reference on employees",ex.Message);
        }

        /**
        There is only one CEO, i.e. only one employee with no manager. 
        */
        [Fact]
        public void ValidateCEO()
        {
            
             string[] employeeCSV =new string[]{ "emp6,emp3,1000",
                                        "emp4,emp3,1000",
                                        "emp1,,2000",      //CEO
                                        "emp2,emp1,1000",
                                        "emp3,emp1,1500",
                                        "emp7,emp6,1000",
                                        "emp8,emp3,1000",
                                        "emp9,,1000"}; //err
                                             


             Exception ex = Assert.Throws<Exception>(() => new Employee(employeeCSV));

            Assert.StartsWith("There are more that one CEOs in the list",ex.Message);
        }
        
        /**
        There is no manager that is not an employee, 
        i.e. all managers are also listed in the employee column.
         */
        [Fact]
        public void ValidateAllManagerAreEmployees()
        {
            
             string[] employeeCSV =new string[]{ "emp6,emp3,1000",
                                        "emp4,emp3,1000",
                                        "emp1,,2000",      //CEO
                                        "emp2,emp1,1000",
                                        "emp3,emp1,1500",
                                        "emp7,emp6,1000",
                                        "emp8,emp3,1000",
                                        "emp9,emp13,1000"}; //err
                                             


             Exception ex = Assert.Throws<Exception>(() => new Employee(employeeCSV));

            Assert.StartsWith("The following managers are not employees",ex.Message);
        }
    

    /*
    */
            [Fact]
        public void ValidateEmployeeReportToOneManager()
        {
            
             string[] employeeCSV =new string[]{ "emp6,emp3,1000",
                                        "emp4,emp3,1000",
                                        "emp1,,2000",      //CEO
                                        "emp2,emp1,1000",
                                        "emp3,emp1,1500",
                                        "emp7,emp6,1000",
                                        "emp8,emp3,1000",
                                        "emp8,emp3,1000"}; //err
                                             


             Exception ex = Assert.Throws<Exception>(() => new Employee(employeeCSV));

            Assert.StartsWith("Employee cannot have more then one manager",ex.Message);
        }
    }
}
