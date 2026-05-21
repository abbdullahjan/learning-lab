// Simple Library Management System
// Demonstrates Struct, Enum, Properties, and Indexers
// Apply structures (struct), enumerations (enum), properties, and indexers in a C# console application to manage library book records efficiently through organized and encapsulated data management techniques. 

using System;

namespace LibraryManagement
{
    // Enumeration
    enum BookStatus
    {
        Available,
        Issued
    }

    // Structure
    struct Book
    {
        private string title;
        private string author;

        // Properties
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public BookStatus Status;
    }

    class Library
    {
        Book[] books = new Book[3];

        // Indexer
        public Book this[int index]
        {
            get { return books[index]; }
            set { books[index] = value; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library lib = new Library();

            // Adding Books
            lib[0] = new Book
            {
                Title = "C# Basics",
                Author = "Abdullah Jan",
                Status = BookStatus.Available
            };

            lib[1] = new Book
            {
                Title = "OOP Concepts",
                Author = "Ahmed",
                Status = BookStatus.Issued
            };

            lib[2] = new Book
            {
                Title = "Data Structures",
                Author = "Abdullah",
                Status = BookStatus.Available
            };

            // Display Books
            Console.WriteLine("===== Library Books =====\n");

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Book " + (i + 1));
                Console.WriteLine("Title  : " + lib[i].Title);
                Console.WriteLine("Author : " + lib[i].Author);
                Console.WriteLine("Status : " + lib[i].Status);
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}