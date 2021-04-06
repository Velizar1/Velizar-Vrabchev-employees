using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Employees
{
    public class EmployeeKeyPair : IEquatable<EmployeeKeyPair>
    {
        public int EmpId1 { get; set; }
        public int EmpId2 { get; set; }

        public bool Equals([AllowNull] EmployeeKeyPair other)
        {
            return (EmpId1 == other.EmpId1 || EmpId1 == other.EmpId2) && (EmpId2 == other.EmpId1 || EmpId2 == other.EmpId2);
        }

        public override int GetHashCode()
        {
            return 17 *(EmpId1.GetHashCode() + EmpId2.GetHashCode());
        }

    }
}
