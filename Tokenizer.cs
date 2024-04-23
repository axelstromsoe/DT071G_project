using System;
using System.Text.RegularExpressions;

namespace SOSCalc
{
    class Tokenizer
    {
        private List<string> tokens = new List<string>();
        private int currentIndex = 1;
        private List<string> specialCharacters = new List<string>(["+", "-", "*", "/", "%", "^", "(", ")", "="]);
        public Tokenizer(string input)
        {
            // Add spaces inbetween individual tokens
            input = Regex.Replace(input, @"(\(|\))", " $1 ");
            input = Regex.Replace(input, @"\+", " + ");
            input = Regex.Replace(input, @"\-", " - ");
            input = Regex.Replace(input, @"\*", " * ");
            input = Regex.Replace(input, @"\/", " / ");
            input = Regex.Replace(input, @"\%", " % ");
            input = Regex.Replace(input, @"\^", " ^ ");
            input = Regex.Replace(input, @"\=", " = ");

            // Split the string into tokens
            string[] triggers = [" "];
            string[] splitedString = input.Split(triggers, StringSplitOptions.RemoveEmptyEntries);

            // Add a start of line token
            tokens.Add("*SOL*");

            int openParentheses = 0;
            int closeParentheses = 0;

            // Add the tokens to the token list and check the amount of open and close parentheses
            foreach (string token in splitedString)
            {
                if (token == "(")
                {
                    openParentheses++;
                }
                else if (token == ")")
                {
                    closeParentheses++;
                }

                tokens.Add(token);
            }

            // Add an end of line token
            tokens.Add("*EOL*");

            // Check if the amount of open and close parentheses match
            if (openParentheses != closeParentheses)
            {
                throw new InvalidStatementException("The statement contains unbalanced parentheses.");
            }
        }

        public void Next()
        {
            // Increase the index
            if (tokens.Count() > currentIndex) { currentIndex++; }
        }

        public string GetCurrent()
        {
            if (currentIndex + 1 == tokens.Count())
            {
                // Return an empty string if the current index has reached EOL
                return " ";
            }
            else
            {
                // Return the current token
                return tokens[currentIndex];
            }
        }

        public string GetPrevious()
        {
            if (currentIndex > 1)
            {
                // Return the previous token
                return tokens[currentIndex - 1];
            }
            else
            {
                // Return the current token if the current index is one
                return tokens[currentIndex];
            }
        }

        public string GetNext()
        {
            if (currentIndex < tokens.Count() - 2)
            {
                // Return the previous token
                return tokens[currentIndex + 1];
            }
            else
            {
                // Return the current token if the current index is one
                return tokens[currentIndex];
            }
        }

        public bool IsNumber()
        {
            // Get the current token
            string currentToken = GetCurrent();

            // Try to parse the current token as a double
            return double.TryParse(currentToken, out _);
        }

        public bool IsName()
        {
            // Get the current token
            string currentToken = GetCurrent();

            // Check if the token is a operator
            if (specialCharacters.Contains(currentToken))
            {
                return false;
            }
            else
            {
                // Try to parse the current token as a float
                return !IsNumber();
            }
        }

        public bool IsAtEnd()
        {
            // Get the current token
            string currentToken = tokens[currentIndex];

            // Check if the token is EOL and return its value
            return currentToken == "*EOL*";
        }

        public bool Find(string token)
        {
            bool success = false;
            Reset();

            // Check if the token exists
            while (!IsAtEnd())
            {
                if (GetCurrent() == token) { success = true; break; }
                Next();
            }

            Reset();
            return success;
        }

        public void Replace(string replacementToken)
        {
            // Replace current token with the given replacement
            tokens[currentIndex] = replacementToken;
        }

        public void Reset()
        {
            // Change the current index value to 1
            currentIndex = 1;
        }
    }
}