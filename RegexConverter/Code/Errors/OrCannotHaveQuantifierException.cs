namespace Siderite.Code.Errors
{
    public class OrCannotHaveQuantifierException : ParseException
    {
        public OrCannotHaveQuantifierException() : base("Quantifier after the | character.")
        {
        }
    }
}