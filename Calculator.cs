using System;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static System.Console;
using System.Security.Cryptography.X509Certificates;

namespace CalculatorSpace
{
    class Calculator
    {
        // PROPERTIES
        private CalcStorage calcStorage = new CalcStorage();

        // METHODS
        public void PrintInterface()
        {
            // Load the functions and variables saved to txt files
            calcStorage.LoadFromFile();

            bool quitting = false;

            Clear();
            WriteLine("\nCALCULATOR");

            while (!quitting)
            {
                // Get the user input
                WriteLine("\nInput:");
                string? input = ReadLine();

                // Tokenize the string input eith the tokenizer class
                Tokenizer tokenizer = new Tokenizer(input);

                if (tokenizer.GetCurrent() == "back")
                {
                    quitting = true;
                }
                else if (tokenizer.GetCurrent() == "clear")
                {
                    Clear();
                    WriteLine("\nCALCULATOR");
                }
                else if (tokenizer.GetCurrent() == "help")
                {
                    Clear();
                    WriteLine("\nCALCULATOR\n");
                    foreach (KeyValuePair<string, string> keyValuePair in calcStorage.GetProtectedWords())
                    {
                        WriteLine($"{keyValuePair.Key.PadRight(10)}: {keyValuePair.Value}");
                    }
                }
                else if (tokenizer.GetCurrent() == "func")
                {
                    Clear();
                    WriteLine("\nCALCULATOR");
                    WriteLine("\n\u001b[47m\u001b[30m Functions \u001b[0m");
                    foreach (KeyValuePair<string, Func<double, double>> function in calcStorage.GetNativeFunctions())
                    {
                        WriteLine($"{function.Key.PadRight(10)}: {nameof(Math)}.{function.Key}");
                    }
                    foreach (MathFunction function in calcStorage.GetCreatedFunctions())
                    {
                        WriteLine($"{function.Name.PadRight(10)}: {function.Value}");
                    }
                }
                else if (tokenizer.GetCurrent() == "vars")
                {
                    Clear();
                    WriteLine("\nCALCULATOR");
                    WriteLine("\n\u001b[47m\u001b[30m Variables \u001b[0m");
                    foreach (MathVariable variable in calcStorage.GetVariables())
                    {
                        WriteLine($"{variable.Name.PadRight(10)}: {variable.Value}");
                    }
                }
                else
                {
                    try
                    {
                        double result = statement(tokenizer);
                        calcStorage.SaveEntry("variable", "ans", result.ToString());
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

        public double statement(Tokenizer tokenizer)
        {
            // Call the assignment method
            double result = assignment(tokenizer);
            // Check if the token is at the end
            if (tokenizer.IsAtEnd())
            {
                return result;
            }
            else
            {
                throw new Exception("Expected end of statement.");
            }
        }

        private double assignment(Tokenizer tokenizer)
        {
            double result = expression(tokenizer);
            while (tokenizer.GetCurrent() == "=")
            {
                tokenizer.Next();
                string name = tokenizer.GetCurrent();
                if (!calcStorage.GetProtectedWords().ContainsKey(name))
                {
                    if (tokenizer.IsName() && name != " ")
                    {
                        calcStorage.SaveEntry("variable", name, result.ToString());
                    }
                    else
                    {
                        throw new Exception("Expected variable name, the name cannot only consist of numbers.");
                    }
                } else
                {
                    throw new Exception($"{name} cannot be used as a variable name.");
                }

                tokenizer.Next();
            }
            return result;
        }

        private double expression(Tokenizer tokenizer)
        {
            double result = term(tokenizer);
            while (tokenizer.GetCurrent() == "+" || tokenizer.GetCurrent() == "-")
            {
                string specialCharacter = tokenizer.GetCurrent();
                tokenizer.Next();
                if (specialCharacter == "+")
                {
                    result += term(tokenizer);
                }
                else
                {
                    result -= term(tokenizer);
                }
            }
            return result;
        }

        private double term(Tokenizer tokenizer)
        {
            double result = factor(tokenizer);

            while (tokenizer.GetCurrent() == "*" || tokenizer.GetCurrent() == "/" || tokenizer.GetCurrent() == "^")
            {
                string specialCharacter = tokenizer.GetCurrent();
                tokenizer.Next();
                double nextFactor = factor(tokenizer);
                if (specialCharacter == "*")
                {
                    result *= nextFactor;
                }
                else if (specialCharacter == "^")
                {
                    result = Math.Pow(result, nextFactor);
                }
                else if (tokenizer.GetCurrent() == "0" || nextFactor == 0)
                {
                    throw new Exception("A value cannot be divided by zero.");
                }
                else
                {
                    result /= nextFactor;
                }
            }
            return result;
        }

        private double factor(Tokenizer tokenizer)
        {
            double result = 0;

            if (tokenizer.GetCurrent() == "(")
            {
                tokenizer.Next();
                result = assignment(tokenizer);
                if (tokenizer.GetCurrent() != ")")
                {
                    throw new Exception("Expected '(' after function name.");
                }
                else
                {
                    tokenizer.Next();
                }
            }
            else if (calcStorage.FunctionExists(tokenizer.GetCurrent()))
            {
                string name = tokenizer.GetCurrent();
                tokenizer.Next();
                if (tokenizer.GetCurrent() == "(")
                {
                    double arg = factor(tokenizer);
                    result = calcStorage.Calculate(name, arg);
                }
                else
                {
                    throw new Exception("Expected '(' after function name.");
                }
            }
            else if (calcStorage.VariableExists(tokenizer.GetCurrent()))
            {
                string name = tokenizer.GetCurrent();
                result = calcStorage.variableValue(name);
                tokenizer.Next();
            }
            else if (tokenizer.IsNumber())
            {
                double.TryParse(tokenizer.GetCurrent(), out double number);
                result = number;
                tokenizer.Next();
            }
            else if (tokenizer.GetCurrent() == "-")
            {
                tokenizer.Next();
                result = -factor(tokenizer);
            }
            else if (tokenizer.IsName())
            {
                string tokenName = tokenizer.GetCurrent();
                tokenizer.Next();
                if (tokenizer.GetCurrent() == "(")
                {
                    throw new Exception($"Cannot find any function with the name '{tokenName}'.");
                }
                else
                {
                    throw new Exception($"Cannot find any variable with the name '{tokenName}'.");
                }
            }
            else
            {
                throw new Exception("Expected a number, name or '('.");
            }
            return result;
        }
    }
}