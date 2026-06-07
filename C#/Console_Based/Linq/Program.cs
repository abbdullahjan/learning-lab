using System;
namespace linqPractice
{
    internal class Linq
    {
        public static void Main(string[] args)
        {
            // SyntaxAndStuff();
            // NestedList();
            // System.Console.WriteLine($"{Console.WriteLine("Hello")}");
            // practice();
            Action<int> myAction = x => System.Console.WriteLine( x * x);

             myAction(2) ;
           
        }

        public static void SyntaxAndStuff()
        {
            List<int> number = new List<int>()
            {
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
            };

            var greaterThan10 = number.Where(n => n > 10);
            // var greaterThan10 = number.Where(Check);

            number.Add(101);

            foreach (var i in greaterThan10)
            {
                System.Console.Write($"{i}, ");
            }

            static bool Check(int n)
            {
                return n > 10;
            }

        }

        //         class Student
        // {
        //     public string Name { get; set; }
        //     public int Marks { get; set; }
        // }
        //         public static void NestedList()
        //         {

        //         List<Student> students =
        //             [
        //                 new Student { Name = "Ali", Marks = 90 },
        //                 new Student { Name = "Sara", Marks = 75 },
        //                 new Student { Name = "Ahmed", Marks = 90 },
        //                 new Student { Name = "Bilal", Marks = 60 },
        //                 new Student { Name = "Usman", Marks = 80 }
        //             ];
        //             var sorted = students.OrderByDescending(s=>s.Marks).Select(s => s.Marks).Distinct();
        //             var thirdMarks = sorted.ElementAt(2);
        //             var thirdPostion = students.Where(s => s.Marks == thirdMarks);
        //             foreach(var i in thirdPostion){
        //             System.Console.WriteLine($"Third: Name{i.Name}, Marks:{i.Marks}");
        //             }
        //             List<List<int>> data =
        //             [
        //                 new List<int> {1,2},
        //                 new List<int> {3,4},
        //                 new List<int> {5,6}
        //             ];
        //             var result = data.Where(n => n[1] > 2);
        //             foreach(var i in result)
        //             {

        //                 System.Console.WriteLine($"{i[0]}, {i[1]}");
        //             }
        //         }

        public class Student
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Department { get; set; }
            public double GPA { get; set; }
        }
        public static void practice()
        {
            var students = new List<Student>
            {
                new Student { Name = "Alice",   Age = 20, Department = "CS",   GPA = 3.8 },
                new Student { Name = "Bob",     Age = 22, Department = "Math", GPA = 3.2 },
                new Student { Name = "Carol",   Age = 21, Department = "CS",   GPA = 3.5 },
                new Student { Name = "David",   Age = 23, Department = "Math", GPA = 2.9 },
                new Student { Name = "Eva",     Age = 20, Department = "CS",   GPA = 3.9 },
                new Student { Name = "Frank",   Age = 24, Department = "Physics", GPA = 3.1 },
                new Student { Name = "Grace",   Age = 22, Department = "Physics", GPA = 3.7 },
                new Student { Name = "Hank",    Age = 21, Department = "Math", GPA = 3.4 },
            };

            // Get all students whose GPA is above 3.5:
            var greaterThan3point5 = students.Where(n => n.GPA > 3.5).Select(n => n.Name);
            foreach (var i in greaterThan3point5)
            {
                System.Console.Write($"{i}, ");
            }


            // Get all CS students who are 21 or older — chaining two conditions:
            var greaterThan21 = students.Where(n => n.Department == "CS" && n.Age >= 21);
            foreach (var i in greaterThan21)
            {
                System.Console.WriteLine($"\n{i.Name} ({i.Age})");
            }
        }
    }

}