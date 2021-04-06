using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Employees
{
    public class EmployeeTableRow 
    {
        public int ProjectID { get; set; }
        public int Emp1Id { get; set; }
        public int Emp2Id { get; set; }
        public DateTime FromEmp1 { get; set; }
        public DateTime ToEmp1 { get; set; }
        public DateTime FromEmp2 { get; set; }
        public DateTime ToEmp2 { get; set; }

        
    }
}
