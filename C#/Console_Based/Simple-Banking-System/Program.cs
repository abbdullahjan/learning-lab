// Simple Banking System
// Demonstrates Exception Handling (try-catch-finally)
// Implement exception handling and debugging techniques in a C# banking system application by validating user input, handling runtime errors using try-catch-finally, and preventing invalid transactions.

using System;

namespace SimpleBankingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            double balance = 1000; // initial balance
            int choice = 0;

            Console.WriteLine("===== SIMPLE BANKING SYSTEM =====");

            while (true)
            {
                try
                {
                    Console.WriteLine("\n1. Check Balance");
                    Console.WriteLine("2. Deposit Money");
                    Console.WriteLine("3. Withdraw Money");
                    Console.WriteLine("4. Exit");

                    Console.Write("\nEnter choice: ");
                    choice = Convert.ToInt32(Console.ReadLine());

                    if (choice == 1)
                    {
                        Console.WriteLine("Current Balance: " + balance);
                    }
                    else if (choice == 2)
                    {
                        Console.Write("Enter deposit amount: ");
                        double deposit = Convert.ToDouble(Console.ReadLine());

                        if (deposit <= 0)
                        {
                            Console.WriteLine("Invalid deposit amount!");
                        }
                        else
                        {
                            balance += deposit;
                            Console.WriteLine("Deposit successful!");
                        }
                    }
                    else if (choice == 3)
                    {
                        Console.Write("Enter withdraw amount: ");
                        double withdraw = Convert.ToDouble(Console.ReadLine());

                        if (withdraw <= 0)
                        {
                            Console.WriteLine("Invalid withdraw amount!");
                        }
                        else if (withdraw > balance)
                        {
                            Console.WriteLine("Insufficient balance!");
                        }
                        else
                        {
                            balance -= withdraw;
                            Console.WriteLine("Withdrawal successful!");
                        }
                    }
                    else if (choice == 4)
                    {
                        Console.WriteLine("Exiting system...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice!");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: Please enter numeric values only.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Error: Number too large!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected error: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("Transaction completed.\n");
                }
            }

            Console.WriteLine("System Closed.");
            Console.ReadKey();
        }
    }
}