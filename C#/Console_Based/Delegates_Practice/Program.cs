// Simple Delegate Example
// Student Result Notification System
// Using Multiple Methods in One Delegate
//  Implement delegates and events in a C# console application by developing a student result notification system that triggers custom events for pass and fail conditions to demonstrate event-driven programming concepts

using System;

namespace DelegateDemo
{
    class Program
    {
        // Declare Delegate
        public delegate void ResultDelegate(string name, int marks);

        // Method 1
        static void ShowStudent(string name, int marks)
        {
            Console.WriteLine("\nStudent Name : " + name);
            Console.WriteLine("Marks        : " + marks);
        }

        // Method 2
        static void CheckResult(string name, int marks)
        {
            Console.WriteLine("In Method 2_CheckResult");
            if (marks >= 50)
            {
                Console.WriteLine("Result       : PASS");
            }
            else
            {
                Console.WriteLine("Result       : FAIL");
            }
        }

        // Method 3
        static void GiveMessage(string name, int marks)
        {
            Console.WriteLine("In Method 3_Give Message");
            if (marks >= 50)
            {
                Console.WriteLine("Message      : Congratulations!");
            }
            else
            {
                Console.WriteLine("Message      : Better Luck Next Time!");
            }
        }

        // Method 4
        static void ShowGrade(string name, int marks)
        {
            Console.WriteLine("In Method 4_Show Grade");
            if (marks >= 80)
            {
                Console.WriteLine("Grade        : A");
            }
            else if (marks >= 60)
            {
                Console.WriteLine("Grade        : B");
            }
            else if (marks >= 50)
            {
                Console.WriteLine("Grade        : C");
            }
            else
            {
                Console.WriteLine("Grade        : F");
            }
        }

        // Method 5
        static void ScholarshipStatus(string name, int marks)
        {
            Console.WriteLine("In Method 5_Scholorship status");
            if (marks >= 85)
            {
                Console.WriteLine("Scholarship  : Eligible");
            }
            else
            {
                Console.WriteLine("Scholarship  : Not Eligible");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("===== Student Result Notification System =====\n");

            Console.Write("Enter Student Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Student Marks: ");
            int marks = Convert.ToInt32(Console.ReadLine());

            // Create Delegate Object
            ResultDelegate result = ShowStudent;

            // Add Multiple Methods
            result += CheckResult;
            result += GiveMessage;
            result += ShowGrade;
            result += ScholarshipStatus;

            // Call Delegate
            result(name, marks);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}