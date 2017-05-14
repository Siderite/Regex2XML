using System;

namespace Siderite.Code.Errors
{
    public class ParseException : Exception
    {
        public ParseException() : this("Error during parsing of regular expression.")
        {
        }

        protected ParseException(string message) : base(message)
        {
        }
    }
}