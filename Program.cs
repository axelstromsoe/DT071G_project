using System;
using static System.Console;

namespace CalculatorSpace
{
    class Program
    {
        static void PrintMenu()
        {
            PrintHeader();
            WriteLine("Choose an option by pressing corresponding keyboard key or toggle between options with the arrow keys followed by enter.\n");

            bool isSelected = false;
            int option = 1;
            int padding = 28;
            string selectedOption = "> \u001b[47m\u001b[30m";

            (int left, int top) = GetCursorPosition();

            ConsoleKeyInfo key;

            while (!isSelected)
            {
                SetCursorPosition(left, top);
                WriteLine($"{(option == 1 ? selectedOption : "  ")}1. Calculator".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 2 ? selectedOption : "  ")}2. Functions".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 3 ? selectedOption : "  ")}3. History".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 4 ? selectedOption : "  ")}X. Quit".PadRight(padding) + "\u001b[0m\n");

                key = ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (option > 1) { option--; }
                        break;
                    case ConsoleKey.DownArrow:
                        if (option < 4) { option++; }
                        break;
                    case ConsoleKey.D1:
                        option = 1;
                        break;
                    case ConsoleKey.D2:
                        option = 2;
                        break;
                    case ConsoleKey.D3:
                        option = 3;
                        break;
                    case ConsoleKey.X:
                        option = 4;
                        break;
                    case ConsoleKey.Enter:
                        isSelected = true;
                        break;
                }
            }

            switch (option)
            {
                case 1:
                    new Calculator().PrintInterface();
                    Clear();
                    PrintMenu();
                    break;
                case 2:
                    new StorageMananger();
                    Clear();
                    PrintMenu();
                    break;
                case 4:
                    WriteLine("Bye!");
                    Thread.Sleep(1000);
                    Clear();
                    break;
            }
        }

        static void PrintHeader()
        {
            string version = "1.0";

            WriteLine($"\nSOS CALC Version {version} {DateTime.Now.ToString("dd-MMMM-yyyy")}");
            WriteLine("(C) Axel Strömsöe, 2024\n");
        }
        static void Main(string[] args)
        {
            Clear();
            PrintMenu();
        }
    }
}