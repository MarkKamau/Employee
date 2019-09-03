using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace employee
{
public class Employee:IEmployee
    {

        public String employeeId{get;set;}
        public String managerId{get;set;}
        public IEnumerable<Employee> employees{get;set;}

        public int salary{get;set;}

        public Employee(){

        }

        public Employee(string[] employeeCSV)
        {
            //convert the csv to an employee list
            IEnumerable<Employee> employees=this.parseEmployeesCSV(employeeCSV);

            //Process the employee hieracy
            Employee employee= GetSubordinates(employees, (new Employee()));

            //Make current object the CEO
            this.employeeId=employee.employeeId;
            this.managerId=employee.managerId;
            this.employees=employee.employees;
            this.salary=employee.salary;
        }
        private IEnumerable<Employee> parseEmployeesCSV(string[] employeeCSV)
        {
            List<Employee> employees=new List<Employee>();
            foreach(String line in employeeCSV)
            {
                string[] values = line.ToString().Split(',');
                        Employee employee=new Employee(){
                            employeeId= (String)values[0], 
                            managerId=(String)values[1],
                            salary=int.Parse(values[2])
                        };

                        ValidateEmployeeReportToOneManager(employees,employee);

                        employees.Add(employee);
            }

            ValidateAllManagerAreEmployees(employees);
            ValidateCEO(employees);
            ValidateCircularReference(employees);
                
            return employees;
        }
        private void ValidateEmployeeReportToOneManager(IEnumerable<Employee> employees, Employee currentEmployee)
        {
            //Employee doesnt report to more than one manager. 
           int emplCount=(from Employee employee in employees
                                    where employee.employeeId==currentEmployee.employeeId
                                    select employee).Count();
            if (emplCount>0)
            {
                throw(new Exception("Employee cannot have more then one manager\n" + currentEmployee.employeeId + "\n"));
            }
        }
        private void ValidateAllManagerAreEmployees(List<Employee> employees)
        {
           IEnumerable<Employee> employeesList=employees;
           IEnumerable<Employee> managers     = employees;

           IEnumerable<Employee> result = employeesList.Where(employee => 
                            managers.All(manager => manager.employeeId != employee.managerId ));

            string nonEmployee="";
            foreach (Employee employee in result)
            {
                if (employee.managerId.ToString()!="")
                    nonEmployee+= ", "+ employee.managerId.ToString();
            }
            if (nonEmployee.Length>0)
            {
                throw(new Exception("The following managers are not employees \n " + nonEmployee + "\n"));
            }                            
        }
        private void ValidateCEO(List<Employee> employees)
        {
           IEnumerable<Employee> ceoList= (from Employee employee in employees
                                                where employee.managerId==""
                                                select employee);

            string sCeo="";
            foreach (Employee ceo in ceoList)
                    sCeo+= ", "+ ceo.employeeId.ToString();

            if (ceoList.Count()>1)
            {
                throw(new Exception("There are more that one CEOs in the list: \n " + sCeo + "\n"));
            }                            
        }
        private void ValidateCircularReference(List<Employee> employees)
        {
            foreach(Employee empl in employees)
            {
                //eliminate the CEO
                if (empl.managerId!=""){ 
                    //Find employees manager
                    Employee manager = (from Employee employee in employees
                                        where employee.employeeId==empl.managerId
                                        select employee).FirstOrDefault();

                    //Does the manager report back to them?
                    if(manager.managerId.Equals(empl.employeeId))
                    {
                        throw(new Exception(string.Format("There is a circular reference on employees \n {0},{1} \n " , manager.employeeId,empl.employeeId )));
                    }
                }
            }   
        }

        private Employee GetSubordinates(IEnumerable<Employee> employees, Employee manager)
        {
            if (manager.employeeId==null)
            {
                //find the CEO
                manager = (from employee in employees where employee.managerId == ""
                        select employee).FirstOrDefault();
            }
            manager.employees=new List<Employee>();
            //Find all subordinate employees if any
            IEnumerable<Employee> subordinates = from employee in employees
                                    where employee.managerId==manager.employeeId
                                    select employee;
        
            manager.employees=subordinates;

            //remove the  employees from the list to make it smaller
            employees = from employee in employees
                                    where employee.managerId!=manager.employeeId
                                    select employee;

            for (int empIndex=0; empIndex<manager.employees.Count(); empIndex++)
            {
               manager.employees.ElementAt(empIndex).GetSubordinates(employees,manager.employees.ElementAt(empIndex));
            }                                    

            return manager;                                    
        }

        public int salaryBudget(Employee manager)
        {
            int salaryBudget=0; 

            if (manager==null)
            {
                throw(new Exception("Employee is not specified"));
            }
            //Add the current managers Salary
            salaryBudget = manager.salary;

            //Sum the salaries of the subordinates in other levels
            if (manager.employees!=null)
            {
                foreach (Employee employee in manager.employees)
                { 
                    salaryBudget += employee.salaryBudget(employee);
                    
                }                                    
            }
            return salaryBudget;                                    
        }
    }
}
