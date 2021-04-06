using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Employees
{
    public class EmployeeRowComparer : IEqualityComparer<EmployeeTableRow>
    {
        public bool Equals([AllowNull] EmployeeTableRow x, [AllowNull] EmployeeTableRow y)
        {
            return ((x.Emp1Id == y.Emp1Id || x.Emp2Id == y.Emp1Id) && (x.Emp1Id == y.Emp2Id || x.Emp2Id == y.Emp2Id) && (x.ProjectID == y.ProjectID) &&
                 (x.FromEmp1 == y.FromEmp1 && x.FromEmp2 == y.FromEmp2 && x.ToEmp1 == y.ToEmp1 && x.ToEmp2 == y.ToEmp2) ||
                 (x.FromEmp2 == y.FromEmp1 && x.FromEmp1 == y.FromEmp2 && x.ToEmp2 == y.ToEmp1 && x.ToEmp1 == y.ToEmp2));
        }

        public int GetHashCode([DisallowNull] EmployeeTableRow obj)
        {
            return 17 *( obj.Emp1Id + obj.Emp2Id+ obj.ProjectID + obj.FromEmp1.Second + obj.FromEmp2.Second + obj.ToEmp1.Second + obj.ToEmp2.Second);
        }
    }
}
