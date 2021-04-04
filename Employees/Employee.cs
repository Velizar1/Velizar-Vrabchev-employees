using System;
using System.Collections.Generic;
using System.Text;

namespace Employees
{
    public class Employee
    {
        public Employee(int id,int projId, DateTime from, DateTime to)
        {
            EmpID = id;
            ProjectID = projId;
            DateFrom = from;
            DateTo = to;
        }
        public int EmpID { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
