using System;
using SOSCalc;
using static System.Console;

namespace SOSCalc
{
    public class UserInterface
    {
        // PROPERTIES
        private Calculator calculator = new Calculator();
        private Storage storage = new Storage();

        // METHODS
        public void Header(string version)
        {
            // Print the header of the program
            WriteLine($"\nSOS CALC Version {version} {DateTime.Now.ToString("dd-MMMM-yyyy")}");
            WriteLine("(C) Axel Strömsöe, 2024\n");
        }

        public int Menu()
        {
            WriteLine("Choose an option by pressing corresponding keyboard key or toggle between options with the arrow keys followed by enter.\n");

            bool isSelected = false;
            int option = 1;
            int padding = 28;
            string selectedOption = "> \u001b[47m\u001b[30m";

            // Get cursorPointPosistion and keyInfo
            (int left, int top) = GetCursorPosition();
            ConsoleKeyInfo key;

            // Print options for the menu
            while (!isSelected)
            {
                SetCursorPosition(left, top);
                WriteLine($"{(option == 1 ? selectedOption : "  ")}1. Calculator".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 2 ? selectedOption : "  ")}2. Functions".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 3 ? selectedOption : "  ")}3. Variables".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 4 ? selectedOption : "  ")}X. Quit".PadRight(padding) + "\u001b[0m\n");

                // Read the keyboard output and change value for option accordingly
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
            return option;
        }

        public string Calculator()
        {
            WriteLine("\nCALCULATOR");

            while (true)
            {
                // Get the user input
                WriteLine("\nInput:");
                string input = ReadLine() ?? "";

                // Tokenize the input with the tokenizer class
                Tokenizer tokenizer = new Tokenizer(input);

                // Evaluate the given value
                if (storage.GetProtectedWords().ContainsKey(tokenizer.GetCurrent()))
                {
                    return tokenizer.GetCurrent();
                }
                else
                {
                    // Try to calculate the given input
                    try
                    {
                        // Calculate the result and save it as ans in the math_variables file
                        double result = calculator.statement(tokenizer);
                        storage.AddEntry("variable", "ans", result.ToString());

                        // Print the result of the calculation
                        WriteLine("Output:");
                        WriteLine(result);
                    }
                    catch (Exception exception)
                    {
                        WriteLine(exception.Message);
                    }
                }
            }
        }

        public void Help()
        {
            bool quitting = false;
            ConsoleKeyInfo key;

            while (!quitting)
            {
                WriteLine("\n\u001b[47m\u001b[30m COMMANDS \u001b[0m\n");

                // Print all protected words
                foreach (KeyValuePair<string, string> protectedWords in storage.GetProtectedWords())
                {
                    WriteLine($"{protectedWords.Key.PadRight(10)}: {protectedWords.Value}");
                }
                WriteLine("\nPress enter to go back");

                // Listen for the enter key to be pressed and end the loop
                key = ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    quitting = true;
                }
                else
                {
                    Clear();
                }
            }
        }

        public int Functions()
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
            return option;
        }

        public void ViewFunctions()
        {
            bool quitting = false;
            ConsoleKeyInfo key;

            while (!quitting)
            {
                // Print all native functions
                WriteLine("\n\u001b[47m\u001b[30m Native Functions \u001b[0m");
                foreach (KeyValuePair<string, Func<double, double>> function in storage.GetNativeFunctions())
                {
                    WriteLine($"{function.Key.PadRight(10)}: {nameof(Math)}.{function.Key}");
                }

                // Print all created functions
                WriteLine("\n\u001b[47m\u001b[30m Created Functions \u001b[0m");
                foreach (StorageEntry function in storage.GetCreatedFunctions())
                {
                    WriteLine($"{function.Name.PadRight(10)}: {function.Definition}");
                }
                WriteLine("\nPress enter to go back");

                // Listen for the enter key to be pressed and end the loop
                key = ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    quitting = true;
                }
                else
                {
                    Clear();
                }
            }
        }

        public void CreateFunction()
        {
            bool quitting = false;

            while (!quitting)
            {
                WriteLine("CREATE FUNCTION\n");

                // Write explanation
                WriteLine("Create a new function by assign it a name and a definition, use 'x' as the unknown constant.");
                WriteLine("The name of the function needs to be one continious word, contain letters and be unique. The");
                WriteLine("definition may use already defined functions and variables native to the system and must contain");
                WriteLine("the unknown constant 'x' atleast once. Write 'back' in the input field to go back.\n");

                bool validName = false;
                bool validDefinition = false;
                string name = "";
                string definition = "";

                // Get user input name
                while (!validName)
                {
                    WriteLine("Function name:");
                    name = ReadLine() ?? "";

                    // Check if the name is valid
                    try
                    {
                        validName = calculator.validateFunctionName(name);
                    }
                    catch (Exception exception)
                    {
                        WriteLine(exception.Message);
                    }

                    // Check if the user wants to go back
                    if (name == "back")
                    {
                        return;
                    }
                }

                // Get the user input definition
                while (!validDefinition)
                {
                    WriteLine("Function definition:");
                    definition = ReadLine() ?? "";

                    // Check if the defintion is valid
                    try
                    {
                        validDefinition = calculator.validateFunctionDefintion(definition);
                    }
                    catch (Exception exception)
                    {
                        WriteLine(exception.Message);
                    }

                    // Check if the user wants to go back
                    if (definition == "back")
                    {
                        return;
                    }
                }

                if (validName && validDefinition)
                {
                    // Save function
                    storage.AddEntry("function", name, definition);
                    WriteLine("Function saved!");
                    quitting = true;
                }
            }
            Thread.Sleep(2000);
        }

        public void DeleteFunction()
        {
            bool isSelected = false;
            int option = 1;
            string selectedOption = "> ";
            StorageList functions = storage.GetCreatedFunctions();

            (int left, int top) = GetCursorPosition();

            ConsoleKeyInfo key;

            while (!isSelected)
            {
                int availableOptions = 1;

                SetCursorPosition(left, top);

                WriteLine("DELETE FUNCTION\n");

                // Print the created functions
                foreach (StorageEntry function in functions)
                {
                    WriteLine($"{(option == availableOptions ? selectedOption : "  ")}{function.Name.PadRight(10)}: {function.Definition}");
                    availableOptions++;
                }
                WriteLine($"{(option == availableOptions ? selectedOption : "  ")}Cancel");

                // Track the keyboard input
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

                // Delete the wanted function
                if (key.Key == ConsoleKey.Enter)
                {
                    if (option != availableOptions)
                    {
                        // Get function
                        StorageEntry function = functions[option - 1];
                        storage.DeleteEntry("function", function.Name);
                        WriteLine("\nFunction deleted!");
                        Thread.Sleep(1000);
                    }
                    isSelected = true;
                }
            }
        }

        public int Variables()
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
                WriteLine($"{(option == 1 ? selectedOption : "  ")}1. View Variables".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 2 ? selectedOption : "  ")}2. Create new variable".PadRight(padding) + "\u001b[0m");
                WriteLine($"{(option == 3 ? selectedOption : "  ")}3. Delete variable".PadRight(padding) + "\u001b[0m");
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
            return option;
        }

        public void ViewVariables()
        {
            bool quitting = false;
            ConsoleKeyInfo key;

            while (!quitting)
            {
                // Print all native variables
                WriteLine("\n\u001b[47m\u001b[30m Native variables \u001b[0m");
                foreach (KeyValuePair<string, double> variable in storage.GetNativeVariables())
                {
                    WriteLine($"{variable.Key.PadRight(10)}: {nameof(Math)}.{variable.Key}");
                }

                // Print all created variable
                WriteLine("\n\u001b[47m\u001b[30m Created Functions \u001b[0m");
                foreach (StorageEntry variable in storage.GetCreatedVariables())
                {
                    WriteLine($"{variable.Name.PadRight(10)}: {variable.Definition}");
                }
                WriteLine("\nPress enter to go back");

                // Listen for the enter key to be pressed and end the loop
                key = ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    quitting = true;
                }
                else
                {
                    Clear();
                }
            }
        }

        public void CreateVariable()
        {
            bool quitting = false;

            while (!quitting)
            {
                WriteLine("CREATE VARIABLE\n");

                // Write explanation
                WriteLine("Create a new variables by assign it a name and a definition. The name of the variable needs to be one");
                WriteLine("continious word, contain letters and be unique. The definition of the variable may use already defined");
                WriteLine("functions and variables native to the system. Write 'back' in the input field to go back.\n");

                bool validName = false;
                bool validDefinition = false;
                string name = "";
                string definition = "";

                // Get user input name
                while (!validName)
                {
                    WriteLine("Variable name:");
                    name = ReadLine() ?? "";

                    // Check if the name is valid
                    try
                    {
                        validName = calculator.validateVariableName(name);
                    }
                    catch (Exception exception)
                    {
                        WriteLine(exception.Message);
                    }

                    // Check if the user wants to go back
                    if (name == "back")
                    {
                        return;
                    }
                }

                // Get the user input definition
                while (!validDefinition)
                {
                    WriteLine("Variable definition:");
                    definition = ReadLine() ?? "";

                    // Check if the defintion is valid
                    try
                    {
                        validDefinition = calculator.validateVariableDefinition(definition);
                    }
                    catch (Exception exception)
                    {
                        WriteLine(exception.Message);
                    }

                    // Check if the user wants to go back
                    if (definition == "back")
                    {
                        return;
                    }
                }

                if (validName && validDefinition)
                {
                    // Calculate definition
                    definition = calculator.statement(new Tokenizer(definition)).ToString();

                    // Save function
                    storage.AddEntry("variable", name, definition);
                    WriteLine("Variable saved!");
                    quitting = true;
                }
            }
            Thread.Sleep(2000);
        }

        public void DeleteVariable()
        {
            bool isSelected = false;
            int option = 1;
            string selectedOption = "> ";
            StorageList variables = storage.GetCreatedVariables();

            (int left, int top) = GetCursorPosition();

            ConsoleKeyInfo key;

            while (!isSelected)
            {
                int availableOptions = 1;

                SetCursorPosition(left, top);

                WriteLine("DELETE VARIABLE\n");

                // Print the created variables
                foreach (StorageEntry variable in variables)
                {
                    WriteLine($"{(option == availableOptions ? selectedOption : "  ")}{variable.Name.PadRight(10)}: {variable.Definition}");
                    availableOptions++;
                }
                WriteLine($"{(option == availableOptions ? selectedOption : "  ")}Cancel");

                // Track the keyboard input
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

                // Delete the wanted variable
                if (key.Key == ConsoleKey.Enter)
                {
                    if (option != availableOptions)
                    {
                        // Get variable
                        StorageEntry variable = variables[option - 1];
                        storage.DeleteEntry("variable", variable.Name);
                        WriteLine("\nVariable deleted!");
                        Thread.Sleep(1000);
                    }
                    isSelected = true;
                }
            }
        }
    }
}