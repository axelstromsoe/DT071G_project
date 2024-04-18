using System;
using static System.Console;

namespace CalculatorSpace
{
    class StorageMananger
    {
        // PROPERTIES
        private CalcStorage calcStorage = new CalcStorage();

        // METHODS
        public StorageMananger()
        {
            // Load the functions and variables saved to txt files
            calcStorage.LoadFromFile();

            // Print options
            Clear();
            PrintOptions();
        }

        public void PrintOptions()
        {
            bool isSelected = false;
            int option = 1;
            int padding = 35;
            string selectedOption = "> \u001b[47m\u001b[30m";

            (int left, int top) = GetCursorPosition();

            ConsoleKeyInfo key;

            while (!isSelected)
            {
                SetCursorPosition(left, top);
                WriteLine($"{(option == 1 ? selectedOption : "  ")}1. View functions".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 2 ? selectedOption : "  ")}2. Create new function".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 3 ? selectedOption : "  ")}3. Delete function".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 4 ? selectedOption : "  ")}X. Go back".PadRight(padding) + "\u001b[0m\n");

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
                    Clear();
                    ViewFunctions();
                    Clear();
                    PrintOptions();
                    break;
                case 2:
                    Clear();
                    CreateFunction();
                    Clear();
                    PrintOptions();
                    break;
                case 3:
                    Clear();
                    DeleteFunction();
                    Clear();
                    PrintOptions();
                    break;
                case 4:
                    Clear();
                    break;
            }
        }

        public void ViewFunctions()
        {
            bool quitting = false;
            ConsoleKeyInfo key;

            while (!quitting)
            {
                WriteLine("\n\u001b[47m\u001b[30m Native Functions \u001b[0m");
                foreach (KeyValuePair<string, Func<double, double>> function in calcStorage.GetNativeFunctions())
                {
                    WriteLine($"{function.Key.PadRight(10)}: {nameof(Math)}.{function.Key}");
                }
                WriteLine("\n\u001b[47m\u001b[30m Created Functions \u001b[0m");
                foreach (MathFunction function in calcStorage.GetCreatedFunctions())
                {
                    WriteLine($"{function.Name.PadRight(10)}: {function.Value}");
                }
                WriteLine("\nPress enter to go back");

                key = ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    quitting = true;
                }
            }
        }

        public void CreateFunction()
        {
            bool quitting = false;

            while (!quitting)
            {
                WriteLine("Create a new function by assign it a name and a definition, use 'x' as the unknown constant.");
                WriteLine("The name of the function needs to be one continious word, contain letters and be unique. The");
                WriteLine("definition may use already defined functions and variables native to the system and must contain");
                WriteLine("the unknown constant 'x' atleast once. Write 'back' in the input field to go back.\n");
                WriteLine("Function name:");
                string? name = ReadLine();
                if (name == "back") { return; }
                WriteLine("Function definition:");
                string? value = ReadLine();
                if (value == "back") { return; }

                if (!calcStorage.FunctionExists(name) && !calcStorage.GetProtectedWords().ContainsKey(name))
                {
                    // Check if the name is valid
                    if (!name.Contains(" ") && !double.TryParse(name, out _))
                    {
                        // Check if the function definition is unvalid
                        if (new Tokenizer(value).ValidFunction(calcStorage.GetNativeFunctions(), calcStorage.GetNativeVariables(), "x"))
                        {
                            calcStorage.SaveEntry("function", name, value);
                            WriteLine("Function saved!");
                            quitting = true;
                        }
                        else
                        {
                            WriteLine("The function definition violates one or more of the rules stated above.");
                        }
                    }
                    else
                    {
                        WriteLine("The function name violates one or more of the rules stated above.");
                    }
                }
                else if (calcStorage.GetProtectedWords().ContainsKey(name))
                {
                    WriteLine($"{name} cannot be used as a function name.");
                }
                else
                {
                    WriteLine($"{name} is already being used as a function name.");
                }
            }
            Thread.Sleep(2000);
        }

        public void DeleteFunction()
        {
            bool isSelected = false;
            int option = 1;
            string selectedOption = "> ";

            (int left, int top) = GetCursorPosition();

            ConsoleKeyInfo key;

            while (!isSelected)
            {
                int availableOptions = 1;
                SetCursorPosition(left, top);

                WriteLine("Delete function\n");
                foreach (MathFunction function in calcStorage.GetCreatedFunctions())
                {
                    WriteLine($"{(option == availableOptions ? selectedOption : "  ")}{function.Name.PadRight(10)}: {function.Value}");
                    availableOptions++;
                }
                WriteLine($"{(option == availableOptions ? selectedOption : "  ")}Cancel");

                key = ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (option > 1) { option--; }
                        break;
                    case ConsoleKey.DownArrow:
                        if (option < availableOptions) { option++; }
                        break;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (option != availableOptions)
                    {
                        calcStorage.GetCreatedFunctions().RemoveAt(option - 1);
                        calcStorage.SaveToFile("math_functions.txt");
                        WriteLine("\nFunction deleted!");
                        Thread.Sleep(1000);
                    }
                    isSelected = true;
                }
            }
        }

        public bool EscapePressed()
        {
            // Return the boolean value of the escape button being pressed at a given moment
            ConsoleKeyInfo key;
            key = ReadKey(true);
            return key.Key == ConsoleKey.Escape;
        }
    }
}