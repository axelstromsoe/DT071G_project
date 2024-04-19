using System;

namespace SOSCalc
{
    // Syntax expetion
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) : base(message)
        {
        }
    }

    // Evaluation expetion
    public class EvaluationException : Exception
    {
        public EvaluationException(string message) : base(message)
        {
        }
    }

    // Not found expetion
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    // Name expetion
    public class InvalidNameException : Exception
    {
        public InvalidNameException(string message) : base(message)
        {
        }
    }

    // Definition expetion
    public class InvalidDefinitionException : Exception
    {
        public InvalidDefinitionException(string message) : base(message)
        {
        }
    }
}