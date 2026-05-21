using System;

namespace TextAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string text;

            Console.WriteLine("===== SIMPLE TEXT ANALYZER =====");
            Console.Write("Enter text: ");
            text = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("1. Word Count");
                Console.WriteLine("2. Search Word");
                Console.WriteLine("3. Replace Word");
                Console.WriteLine("4. Reverse Text");
                Console.WriteLine("5. Palindrome Check");
                Console.WriteLine("6. Exit");

                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 1)
                {
                    string[] words = text.Split(' ');
                    Console.WriteLine("Word Count: " + words.Length);
                }

                else if (choice == 2)
                {
                    Console.Write("Enter word: ");
                    string word = Console.ReadLine();

                    if (text.Contains(word))
                        Console.WriteLine("Found!");
                    else
                        Console.WriteLine("Not Found!");
                }

                else if (choice == 3)
                {
                    Console.Write("Old word: ");
                    string oldWord = Console.ReadLine();

                    Console.Write("New word: ");
                    string newWord = Console.ReadLine();

                    text = text.Replace(oldWord, newWord);

                    Console.WriteLine("Updated: " + text);
                }

                else if (choice == 4)
                {
                    char[] arr = text.ToCharArray();
                    Array.Reverse(arr);

                    Console.WriteLine("Reversed: " + new string(arr));
                }

                else if (choice == 5)
                {
                    // Convert to lowercase first
                    string temp = text.ToLower();

                    // Remove spaces manually (important step)
                    string cleaned = "";
                    foreach (char c in temp)
                    {
                        if (c != ' ')
                            cleaned += c;
                    }

                    // Reverse cleaned string
                    char[] arr = cleaned.ToCharArray();
                    Array.Reverse(arr);
                    string reversed = new string(arr);

                    // Compare
                    if (cleaned == reversed)
                        Console.WriteLine("Palindrome!");
                    else
                        Console.WriteLine("Not Palindrome!");
                }

                else if (choice == 6)
                {
                    break;
                }

                else
                {
                    Console.WriteLine("Invalid choice!");
                }
            }
        }
    }
}