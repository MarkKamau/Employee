using System;
using System.Collections.Generic;

namespace employee
{
    public interface IEmployee
    {

        String employeeId{get;set;}
        String managerId{get;set;}
        IEnumerable<Employee> employees{get;set;}

        int salaryBudget(Employee manager);

        int salary{get;set;}
    }
}