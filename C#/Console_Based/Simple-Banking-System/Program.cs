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
            double balance = 1000;
            int choice = 0;

            Console.WriteLine("===== SIMPLE BANKING SYSTEM =====");

            while (true)
            {
                try
                {
                    Console.WriteLine("\n1. Check Balance");
                    Console.WriteLine("2. Deposit Money");
                    Console.WriteLine("3. Withdraw Money");
                    Console.WriteLine("4. Split Money Among People");
                    Console.WriteLine("5. Exit");

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
                        SplitMoney(ref balance);
                    }
                    else if (choice == 5)
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
                    Console.WriteLine("\n==================================");
                    Console.WriteLine("           INPUT ERROR            ");
                    Console.WriteLine("==================================");
                    Console.WriteLine(" Please enter numeric values only ");
                    Console.WriteLine("==================================\n");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("\n==================================");
                    Console.WriteLine("         OVERFLOW ERROR           ");
                    Console.WriteLine("==================================");
                    Console.WriteLine(" Number is too large to process   ");
                    Console.WriteLine("==================================\n");
                }
                catch (DivideByZeroException)
                {
                    Console.WriteLine("\n==================================");
                    Console.WriteLine("       DIVIDE BY ZERO ERROR       ");
                    Console.WriteLine("==================================");
                    Console.WriteLine(" Number of people cannot be zero  ");
                    Console.WriteLine("==================================\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n==================================");
                    Console.WriteLine("        SYSTEM ERROR              ");
                    Console.WriteLine("==================================");
                    Console.WriteLine(" " + ex.Message);
                    Console.WriteLine("==================================\n");
                }
                finally
                {
                    Console.WriteLine("Transaction completed.\n");
                }
            }

            Console.WriteLine("System Closed.");
            Console.ReadKey();
        }

        // ---------------- SPLIT MONEY FUNCTION ----------------
        static void SplitMoney(ref double balance)
        {
            Console.Write("Enter total amount to split: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter number of people: ");
            int people = Convert.ToInt32(Console.ReadLine());

            if (people == 0)
            {
                throw new DivideByZeroException();
            }

            if (people < 0)
            {
                throw new Exception("Number of people cannot be negative.");
            }

            if (amount <= 0)
            {
                Console.WriteLine("Invalid amount!");
                return;
            }

            if (amount > balance)
            {
                Console.WriteLine("Insufficient balance!");
                return;
            }

            double share = amount / people;
            balance -= amount;

            Console.WriteLine($"Each person gets: {share}");
            Console.WriteLine("Money split successfully!");
        }
    }
}