using System;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SOSCalc;
using System.Xml.XPath;

namespace SOSCalc
{
    // class Calculator
    class Calculator
    {
        // PROPERTIES
        private Storage storage = new Storage();

        // METHODS
        public double statement(Tokenizer tokenizer)
        {
            // Reload the json files
            storage.LoadFromFile();

            // Call the assignment method
            double result = assignment(tokenizer);

            // Check if the token is at the end
            if (tokenizer.IsAtEnd())
            {
                return result;
            }
            else
            {
                throw new SyntaxException("Expected end of statement.");
            }
        }

        private double assignment(Tokenizer tokenizer)
        {
            // Call the assignment method
            double result = expression(tokenizer);

            // Check if an equal sign exists
            while (tokenizer.GetCurrent() == "=")
            {
                tokenizer.Next();
                string name = tokenizer.GetCurrent();

                // Assign a value to the given variable name if the name is not in use
                if (!storage.GetProtectedWords().ContainsKey(name))
                {
                    if (tokenizer.IsName())
                    {
                        storage.AddEntry("variable", name, result.ToString());
                    }
                    else
                    {
                        throw new SyntaxException("Expected variable name, the name cannot only consist of numbers.");
                    }
                }
                else
                {
                    throw new InvalidNameException($"{name} cannot be used as a variable name.");
                }

                tokenizer.Next();
            }
            return result;
        }

        private double expression(Tokenizer tokenizer)
        {
            // CAll the term method
            double result = term(tokenizer);

            // Check if a plus or minus sign exists
            while (tokenizer.GetCurrent() == "+" || tokenizer.GetCurrent() == "-")
            {
                string specialCharacter = tokenizer.GetCurrent();
                tokenizer.Next();

                // Add or subtract the two terms
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
            // Call the factor method
            double result = factor(tokenizer);

            // Check if a multiply, division or exponent sign exists
            while (tokenizer.GetCurrent() == "*" || tokenizer.GetCurrent() == "/" || tokenizer.GetCurrent() == "^")
            {
                string specialCharacter = tokenizer.GetCurrent();
                tokenizer.Next();
                double nextFactor = factor(tokenizer);

                // Multiply, divide or raise to an exponent the two factors
                if (specialCharacter == "*")
                {
                    result *= nextFactor;
                }
                else if (specialCharacter == "^")
                {
                    result = Math.Pow(result, nextFactor);
                }
                // Stop the calculator from dividing something with zero
                else if (tokenizer.GetCurrent() == "0" || nextFactor == 0)
                {
                    throw new EvaluationException("A value cannot be divided by zero.");
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

            // Check if the current token is a parentheses
            if (tokenizer.GetCurrent() == "(")
            {
                tokenizer.Next();

                // Call the assignment method for whatever is inside the parentheses
                result = assignment(tokenizer);

                // Check if the end of the statement contains a closing parentheses
                if (tokenizer.GetCurrent() != ")")
                {
                    throw new SyntaxException("Expected '(' after function name.");
                }
                else
                {
                    tokenizer.Next();
                }
            }
            // Check if the current token is a variable
            else if (storage.VariableExists(tokenizer.GetCurrent()) && tokenizer.GetNext() != "(")
            {
                string name = tokenizer.GetCurrent();

                // Get the variable definition
                result = storage.variableDefinition(name);
                tokenizer.Next();
            }
            // Check if the current token is a function
            else if (storage.FunctionExists(tokenizer.GetCurrent()))
            {
                string name = tokenizer.GetCurrent();
                tokenizer.Next();

                // Get the function argument
                if (tokenizer.GetCurrent() == "(")
                {
                    double arg = factor(tokenizer);
                    result = CalculateFunctionValue(name, arg);
                }
                else
                {
                    throw new SyntaxException("Expected '(' after function name.");
                }
            }
            // Check if the current token is a number
            else if (tokenizer.IsNumber())
            {
                // Try to parse the token to a double and return the corresponding value
                double.TryParse(tokenizer.GetCurrent(), out double number);
                result = number;
                tokenizer.Next();
            }
            // Check if the current token is a minus sign
            else if (tokenizer.GetCurrent() == "-")
            {
                tokenizer.Next();

                // Return the negative equivalent of the remaining tokens
                result = -factor(tokenizer);
            }
            // Check if the current token is a name
            else if (tokenizer.IsName())
            {
                string tokenName = tokenizer.GetCurrent();
                tokenizer.Next();

                // Figure out the current token is suppose to be a name or function based the apperence of parentheses
                if (tokenizer.GetCurrent() == "(")
                {
                    throw new NotFoundException($"Cannot find any function with the name '{tokenName}'.");
                }
                else
                {
                    throw new NotFoundException($"Cannot find any variable with the name '{tokenName}'.");
                }
            }
            else
            {
                throw new SyntaxException("Expected a number, name or '('.");
            }
            return result;
        }

        private double CalculateFunctionValue(string name, double arg)
        {
            StorageList createdFunctions = storage.GetCreatedFunctions();
            Dictionary<string, Func<double, double>> nativeFunctions = storage.GetNativeFunctions();
            double result = 0;

            // Try to get the wanted function
            StorageEntry? function = createdFunctions.Find(v => v.Name == name);

            // Check if the function exists in the createdFunction list
            if (function != null)
            {
                // Create a tokenizer with the function definition
                Tokenizer tokenizer = new Tokenizer(function.Definition);

                // Parse the function value
                while (!tokenizer.IsAtEnd())
                {
                    string currentToken = tokenizer.GetCurrent();

                    // Check if the current token is x
                    if (currentToken == "x")
                    {
                        // Change x to the given value 
                        tokenizer.Replace(arg.ToString());
                    }
                    tokenizer.Next();
                }
                // Reset the token
                tokenizer.Reset();

                // Calculate the result of the parsed tokenizer
                result = statement(tokenizer);
            }
            // Check if the function exists in the nativeFunction list
            else if (nativeFunctions.ContainsKey(name))
            {
                // Assign the value of f(value) as the result
                result = nativeFunctions[name](arg);
            }
            // Alert an not found error
            else
            {
                throw new NotFoundException($"Cannot find any function with the name '{name}'.");
            }

            // Check if the function
            return result;
        }

        public bool validateFunctionName(string name)
        {
            // Reload the storage
            storage.LoadFromFile();

            // Check if the name is an empty string 
            if (name == "")
            {
                throw new InvalidNameException("The name cannot be an empty string.");
            }
            // Check if the name is already in use
            else if (storage.FunctionExists(name))
            {
                throw new InvalidNameException($"{name} is already used for another function.");
            }
            // Check if the name is a protected word
            else if (storage.GetProtectedWords().ContainsKey(name))
            {
                throw new InvalidNameException($"{name} is a protected word and can therefore not be used as a function name.");
            }
            // Check if the name contains a space
            else if (name.Contains(" "))
            {
                throw new InvalidNameException("The name cannot contain any spaces.");
            }
            // Check if the name is only numbers
            else if (double.TryParse(name, out _))
            {
                throw new InvalidNameException("The name cannot only consist of numbers");
            }
            else
            {
                return true;
            }
        }

        public bool validateFunctionDefintion(string definition)
        {
            Tokenizer tokenizer = new Tokenizer(definition);

            // Reload the storage
            storage.LoadFromFile();

            while (!tokenizer.IsAtEnd())
            {
                string currentToken = tokenizer.GetCurrent();

                // Check if the token contains any unknown variables
                if (tokenizer.IsName() && currentToken != "x" && !storage.GetNativeFunctions().ContainsKey(currentToken) && !storage.GetNativeVariables().ContainsKey(currentToken))
                {
                    throw new InvalidDefinitionException("The function defintion cannot contain unknown variables.");
                }
                tokenizer.Next();
            }

            // Check if the definition use the unknown constant
            if (!tokenizer.Find("x"))
            {
                throw new InvalidDefinitionException("The function do not contain the unknown constant x.");
            }
            return true;
        }

        public bool validateVariableName(string name)
        {
            // Reload the storage
            storage.LoadFromFile();

            // Check if the name is an empty string 
            if (name == "")
            {
                throw new InvalidNameException("The name cannot be an empty string.");
            }
            // Check if the name is already being used
            else if (storage.VariableExists(name))
            {
                throw new InvalidNameException($"{name} is already used for another variable.");
            }
            // Check if the name is a protected word
            else if (storage.GetProtectedWords().ContainsKey(name))
            {
                throw new InvalidNameException($"{name} is a protected word and can therefore not be used as a variable name.");
            }
            // Check if the name contains a space
            else if (name.Contains(" "))
            {
                throw new InvalidNameException("The name cannot contain any spaces.");
            }
            // Check if the name is only numbers
            else if (double.TryParse(name, out _))
            {
                throw new InvalidNameException("The name cannot only consist of numbers");
            }
            else
            {
                return true;
            }
        }

        public bool validateVariableDefinition(string definition)
        {
            Tokenizer tokenizer = new Tokenizer(definition);

            // Reload the storage
            storage.LoadFromFile();

            while (!tokenizer.IsAtEnd())
            {
                string currentToken = tokenizer.GetCurrent();

                // Check if the token contains any unknown variables
                if (tokenizer.IsName() && currentToken != "x" && !storage.GetNativeFunctions().ContainsKey(currentToken) && !storage.GetNativeVariables().ContainsKey(currentToken))
                {
                    throw new InvalidDefinitionException("The variable definition cannot contain unknown variables.");
                }
                tokenizer.Next();
            }

            // Try to calculate the variable definition
            tokenizer.Reset();
            double? result = statement(tokenizer);

            return result != null;
        }
    }
}