// This is a class Practice Problem, given as assignment
// Apply arrays, strings, and List<T> collection in a C# console application to manage Employee records with Add, Search, Update, Delete, and Display operations through a menu-driven interface, and demonstrate the advantages of generic collections over traditional arrays in modern C# applications.


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
namespace EmployeeRecord
{
    public class Employee
    {
        public int id;
        public string name;
        public string[] skills;

        public Employee(int id, string name, string[] skills)
        {
            this.name = name;
            this.id = id;
            this.skills = skills;
        }
    }

    public class EmployeeRecordSystem
    {
        

    static void Main(string[] args)
    {
        List<Employee> Employees = new List<Employee>();

        while (true)
        {
            Console.WriteLine("\n===== Employee Record System =====");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Display All Employees");
            Console.WriteLine("3. Search Employee");
            Console.WriteLine("4. Update Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddEmployee(Employees);
                    break;

                case 2:
                    DisplayEmployees(Employees);
                    break;

                case 3:
                    SearchEmployee(Employees);
                    break;

                case 4:
                    UpdateEmployee(Employees);
                    break;

                case 5:
                    DeleteEmployee(Employees);
                    break;

                case 6:
                    Console.WriteLine("Exiting program...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }    

    static int idCount = -1 ;
    static void AddEmployee(List<Employee> e)
        {
            idCount++;

            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "Unknown";

            string[] skills = new string[3];

            Console.WriteLine("Enter 3 Skills:");

            for (int i = 0; i < 3; i++)
            {
                Console.Write($"Skill {i + 1}: ");
                skills[i] = Console.ReadLine() ?? "None";
            }

            e.Add(new Employee(idCount, name, skills));

            Console.WriteLine("Employee added successfully!");
        }   
    

    // ---------------- DISPLAY ----------------
        static void DisplayEmployees(List<Employee> e)
        {
            Console.WriteLine("\n--- Employee List ---");
            if(e.Count == 0)
            {
                Console.WriteLine("List is empty");
                return;
            }

            foreach (var emp in e)
            {
                Console.WriteLine($"\nID: {emp.id}");
                Console.WriteLine($"Name: {emp.name}");
                Console.WriteLine("Skills:");

                foreach (var s in emp.skills)
                {
                    Console.WriteLine("- " + s);
                }
            }
        }

        // ---------------- SEARCH ----------------
        static void SearchEmployee(List<Employee> e)
        {
            Console.Write("Enter ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            foreach (var emp in e)
            {
                if (emp.id == id)
                {
                    Console.WriteLine($"Found: {emp.name}");
                    return;
                }
            }

            Console.WriteLine("Employee not found.");
        }

        // ---------------- UPDATE ----------------
        static void UpdateEmployee(List<Employee> e)
        {
            Console.Write("Enter ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            foreach (var emp in e)
            {
                if (emp.id == id)
                {
                    Console.Write("New Name: ");
                    emp.name = Console.ReadLine() ?? "Guest";

                    Console.WriteLine("Update Skills:");

                    for (int i = 0; i < emp.skills.Length; i++)
                    {
                        Console.Write($"Skill {i + 1}: ");
                        emp.skills[i] = Console.ReadLine() ?? "none";
                    }

                    Console.WriteLine("Updated successfully!");
                    return;
                }
            }

            Console.WriteLine("Employee not found.");
        }

        // ---------------- DELETE ----------------
        static void DeleteEmployee(List<Employee> e)
        {
            Console.Write("Enter ID: ");
            int id = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < e.Count; i++)
            {
                if (e[i].id == id)
                {
                    e.RemoveAt(i);
                    Console.WriteLine("Deleted successfully!");
                    return;
                }
            }

            Console.WriteLine("Employee not found.");
        }
    }


}