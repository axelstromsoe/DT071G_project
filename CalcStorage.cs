using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;

namespace CalculatorSpace
{
    class MathFunction
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    class MathVariable
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    class CalcStorage
    {
        // PROPERTIES
        private List<MathFunction> createdFunctions = new List<MathFunction>();
        private Dictionary<string, Func<double, double>> nativeFunctions = new Dictionary<string, Func<double, double>>
        {
            {"log", Math.Log},
            {"exp", Math.Exp},
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"sqrt", Math.Sqrt}
        };
        private List<MathVariable> createdVariables = new List<MathVariable>();
        private Dictionary<string, double> nativeVariables = new Dictionary<string, double>
        {
            {"PI", Math.PI},
            {"E", Math.E}
        };
        private Dictionary<string, string> protectedWords = new Dictionary<string, string>
        {
            {"func", "view functions"},
            {"vars", "view variables"},
            {"clear", "clear the current calculations"},
            {"back", "go back to the menu"},
            {"help", "get help"}
        };
        private string FunctionFilePath = "math_functions.txt";
        private string VariablesFilePath = "math_variables.txt";

        // METHODS
        public List<MathFunction> GetCreatedFunctions()
        {
            return createdFunctions;
        }
        public Dictionary<string, Func<double, double>> GetNativeFunctions()
        {
            return nativeFunctions;
        }

        public List<MathVariable> GetVariables()
        {
            return createdVariables;
        }

        public Dictionary<string, double> GetNativeVariables()
        {
            return nativeVariables;
        }

        public Dictionary<string, string> GetProtectedWords()
        {
            return protectedWords;
        }

        public void SaveEntry(string type, string name, string value)
        {
            switch (type)
            {
                case "function":
                    createdFunctions.Add(new MathFunction { Name = name, Value = value });
                    SaveToFile(FunctionFilePath);
                    break;
                case "variable":
                    if (VariableExists(name))
                    {
                        createdVariables.Find(v => v.Name == name).Value = value;
                    }
                    else
                    {
                        createdVariables.Add(new MathVariable { Name = name, Value = value });
                    }
                    SaveToFile(VariablesFilePath);
                    break;
            }

        }

        public void LoadFromFile()
        {
            if (File.Exists(FunctionFilePath))
            {
                string[] lines = File.ReadAllLines(FunctionFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        createdFunctions.Add(new MathFunction { Name = parts[0], Value = parts[1] });
                    }
                }
            }
            if (File.Exists(VariablesFilePath))
            {
                string[] lines = File.ReadAllLines(VariablesFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        createdVariables.Add(new MathVariable { Name = parts[0], Value = parts[1] });
                    }
                }
            }
        }

        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                switch (filePath)
                {
                    case "math_functions.txt":
                        foreach (MathFunction function in createdFunctions)
                        {
                            writer.WriteLine($"{function.Name}|{function.Value}");
                        }
                        break;
                    case "math_variables.txt":
                        foreach (MathVariable variable in createdVariables)
                        {
                            writer.WriteLine($"{variable.Name}|{variable.Value}");
                        }
                        break;
                }
            }
        }

        public bool FunctionExists(string name)
        {
            MathFunction? createdFunction = createdFunctions.Find(f => f.Name == name);
            return createdFunction != null || nativeFunctions.ContainsKey(name);
        }

        public bool VariableExists(string name)
        {
            MathVariable? variable = createdVariables.Find(f => f.Name == name);
            return variable != null || nativeVariables.ContainsKey(name);
        }

        public double Calculate(string name, double x)
        {
            MathFunction? function = createdFunctions.Find(f => f.Name == name);
            double result = 0;

            // Check if the function exists i the created or native functions list / dictionary
            if (function != null)
            {
                // Create a tokenizer and calculate the result using the calculator class
                Tokenizer tokenizer = createParsedToken(function.Value, x.ToString());
                result = new Calculator().statement(tokenizer);
            }
            else if (nativeFunctions.ContainsKey(name))
            {
                // Assign the value of f(x) as the result
                result = nativeFunctions[name](x);
            }
            return result;
        }

        private Tokenizer createParsedToken(string functionValue, string x)
        {
            // Create a tokenizer with the function value
            Tokenizer tokenizer = new Tokenizer(functionValue);

            // Parse the function value
            while (!tokenizer.IsAtEnd())
            {
                string currentToken = tokenizer.GetCurrent();

                // Check if the current token is x
                if (currentToken == "x")
                {
                    // Change x to the given value 
                    tokenizer.Replace(x);
                }
                tokenizer.Next();
            }
            // Reset the token
            tokenizer.Reset();

            return tokenizer;
        }

        public double variableValue(string name)
        {
            MathVariable? variable = createdVariables.Find(f => f.Name == name);
            double value = 0;

            if (variable != null)
            {
                double.TryParse(variable.Value, out value);
            }
            else
            {
                value = nativeVariables[name];
            }


            return value;
        }
    }
}