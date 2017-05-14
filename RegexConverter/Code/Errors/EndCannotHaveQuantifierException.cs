namespace Siderite.Code.Errors
{
    public class EndCannotHaveQuantifierException : ParseException
    {
        public EndCannotHaveQuantifierException() : base("Quantifier after the $ character.")
        {
        }
    }
}