using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Employees
{
    public class AppController
    {

        public AppController()
        {
            Employees = new List<Employee>();
        }
        public List<Employee> Employees { get; set; }

        public void ExtractDataFromFile(string file)
        {
            try
            {
                using (var streamReader = new StreamReader(file))
                {
                    while (!streamReader.EndOfStream)
                    {
                        MapData(streamReader.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void FindLongestCollaboratingEmployees()
        {
            Dictionary<EmployeeKeyPair, TimeSpan> result = WorkingHoursOfEachPairOfEmployeesWithTheSameProjects();

            int firstEmpId, secondEmpId;
            decimal maxTimeWorkedTogether;
            try
            {
                var longestCoworkers = result
                    .Select(kv => new
                    {
                        pair = kv.Key,
                        totalWorkTime = kv.Value.TotalSeconds
                    })
                    .OrderByDescending(v => v.totalWorkTime)
                    .FirstOrDefault();

                (firstEmpId, secondEmpId, maxTimeWorkedTogether) = (longestCoworkers.pair.EmpId1, longestCoworkers.pair.EmpId2, (decimal)longestCoworkers.totalWorkTime);
                Console.WriteLine($"{firstEmpId} and {secondEmpId} has worked longer together for : {maxTimeWorkedTogether} seconds");
            }
            catch (Exception e)
            {
                Console.WriteLine("No employees found.");
            }

        }

        private Dictionary<EmployeeKeyPair, TimeSpan> WorkingHoursOfEachPairOfEmployeesWithTheSameProjects()
        {
            var employeesWhoHadWorkedTogether = new Dictionary<EmployeeKeyPair, TimeSpan>();
            var joinEmployees = Employees.Join(Employees, e1 => e1.ProjectID, e2 => e2.ProjectID, (e1, e2) =>
                   {
                       if (e1.EmpID != e2.EmpID)
                       {
                           return new EmployeeTableRow
                           {

                               ProjectID = e1.ProjectID,
                               Emp1Id = e1.EmpID,
                               Emp2Id = e2.EmpID,
                               FromEmp1 = e1.DateFrom,
                               ToEmp1 = e1.DateTo,
                               FromEmp2 = e2.DateFrom,
                               ToEmp2 = e2.DateTo
                           };
                       }
                       return null;
                   })
                .Where(r => r != null)
                .Distinct(new EmployeeRowComparer())
                .ToList();


            for (int i = 0; i < joinEmployees.Count; i++)
            {
                var empOriginalPair = new EmployeeKeyPair()
                {
                    EmpId1 = joinEmployees[i].Emp1Id,
                    EmpId2 = joinEmployees[i].Emp2Id
                };
                var empSwitchedPair = new EmployeeKeyPair()
                {
                    EmpId2 = joinEmployees[i].Emp1Id,
                    EmpId1 = joinEmployees[i].Emp2Id
                };
                var timeSpendOnProject = GetTimeSpentOnProject(joinEmployees[i].FromEmp1, joinEmployees[i].ToEmp1, joinEmployees[i].FromEmp2, joinEmployees[i].ToEmp2);
                if (timeSpendOnProject == null)
                    continue;

                if (employeesWhoHadWorkedTogether.ContainsKey(empOriginalPair))
                    employeesWhoHadWorkedTogether[empOriginalPair] += (TimeSpan)timeSpendOnProject;
                else if (employeesWhoHadWorkedTogether.ContainsKey(empSwitchedPair))
                    employeesWhoHadWorkedTogether[empSwitchedPair] += (TimeSpan)timeSpendOnProject;
                else
                {
                    employeesWhoHadWorkedTogether.Add(empOriginalPair, (TimeSpan)timeSpendOnProject);
                }
            }

            return employeesWhoHadWorkedTogether;
        }

        private TimeSpan? GetTimeSpentOnProject(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
        {

            var biggerStart = new DateTime(Math.Max(firstStart.Ticks, secondStart.Ticks));
            var smallerEnd = new DateTime(Math.Min(firstEnd.Ticks, secondEnd.Ticks));
            var workedTime = smallerEnd - biggerStart;
            return workedTime.TotalSeconds < 0 ? (TimeSpan?)null : workedTime;
        }

        private void MapData(string data)
        {
            var information = data.Split(", ");
            if (IsValid(information))
            {
                Employees.Add(
                    new Employee(
                    int.Parse(information[0]),
                    int.Parse(information[1]),
                    DateTime.Parse(information[2]),
                    DateTime.Parse(information[3].Equals("NULL") ? DateTime.Now.ToString() : information[3])
                    ));
            }
            else
            {
                Console.WriteLine($"Expected data id([1-9]*), id([1-9]*), date(yyy-mm-dd), date(yyyy-mm-dd | NULL). Found : {data}");
            }
        }

        private bool IsValid(string[] information)
        {
            if (information.Length != 4)
                return false;
            var regex = new Regex(@"^[1-9]*[0-9]*$");
            var isCorrectFirstId = regex.IsMatch(information[0]);
            var isCorrectsecondId = regex.IsMatch(information[1]);

            regex = new Regex(@"^[1-9]{1}[0-9]{3}-[0-9]{2}-[0-9]{2}$");
            var isCorrectFromDate = regex.IsMatch(information[2]);
            var isCorrectToDate = regex.IsMatch(information[3]);
            isCorrectToDate = (!isCorrectToDate ? information[3].Equals("NULL") : isCorrectToDate);
            return isCorrectFirstId && isCorrectsecondId && isCorrectFromDate && isCorrectToDate;
        }
    }
}
