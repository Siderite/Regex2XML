namespace Siderite.Code.Errors
{
    public class InvalidNumericQuantifierException : ParseException
    {
        public InvalidNumericQuantifierException() : base("Invalid numeric quantifier.")
        {
        }
    }
}