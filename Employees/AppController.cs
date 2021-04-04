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
        public void FindLongestCollaboratingEmpleyees()
        {
            Dictionary<(int empId1, int empId2, int projId), TimeSpan> result = WorkingHoursOfEachPairOfEmployeesWithTheSameProjects();

            int firstEmpId, secondEmpId;
            decimal maxTimeWorkedTogether;
            try
            {
                var longestCoworkers = result
                   .GroupBy(g => new
                   {
                       g.Key.empId1,
                       g.Key.empId2,
                   })
                   .Select(x => new
                   {
                       x.Key.empId1,
                       x.Key.empId2,
                       total = (decimal)x.Sum(t => t.Value.TotalSeconds)
                   })
                   .OrderByDescending(g => g.total)
                   .FirstOrDefault();

                (firstEmpId, secondEmpId, maxTimeWorkedTogether) = (longestCoworkers.empId1, longestCoworkers.empId2, longestCoworkers.total);
                Console.WriteLine($"{firstEmpId} and {secondEmpId} has worked longer together for : {maxTimeWorkedTogether} seconds");
            }
            catch (Exception)
            {
                Console.WriteLine("No employees found.");
            }

        }

        private Dictionary<(int empId1, int empId2, int projId), TimeSpan> WorkingHoursOfEachPairOfEmployeesWithTheSameProjects()
        {
            var result = new Dictionary<(int empId1, int empId2, int projId), TimeSpan>();
            for (int i = 0; i < Employees.Count - 1; i++)
            {
                for (int j = i + 1; j < Employees.Count; j++)
                {

                    foreach (var projectId in Employees.Select(e => e.ProjectID))
                    {
                        if (Employees[i].EmpID == Employees[j].EmpID)
                            continue;
                        var currTuple = (Employees[i].EmpID, Employees[j].EmpID, projectId);
                        if (result.ContainsKey(currTuple))
                            continue;
                        var timeWorkedTogether = GetTimeSpentOnProject(Employees[i].EmpID, Employees[j].EmpID, projectId, Employees);

                        if (timeWorkedTogether == null)
                            continue;
                        result[currTuple] = timeWorkedTogether.Value;
                    }
                }
            }

            return result;
        }

        private TimeSpan? GetTimeSpentOnProject(int empId1, int empId2, int projId, IEnumerable<Employee> employeeCollection)
        {
            var poeopleWhoHaveWorkedOnTheProject = employeeCollection
                .Where(e => e.ProjectID == projId);
            var firstPerson = poeopleWhoHaveWorkedOnTheProject.FirstOrDefault(p => p.EmpID == empId1);
            var secondPerson = poeopleWhoHaveWorkedOnTheProject.FirstOrDefault(p => p.EmpID == empId2);
            if (firstPerson == null || secondPerson == null)
                return null;
            var biggerStart = new DateTime(Math.Max(firstPerson.DateFrom.Ticks, secondPerson.DateFrom.Ticks));
            var smallerEnd = new DateTime(Math.Min(firstPerson.DateTo.Ticks, secondPerson.DateTo.Ticks));
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
                Console.WriteLine($"Expected data id([1-9]*), id([1-9]*), date(yyy-mm-dd), date(yyy-mm-dd | NULL). Found : {data}");
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
