using System;

namespace Employees
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file full path+name :");
            var filename = Console.ReadLine();
            AppController controller = new AppController();
            controller.ExtractDataFromFile(filename);
            controller.FindLongestCollaboratingEmpleyees();
        }
    }
}
