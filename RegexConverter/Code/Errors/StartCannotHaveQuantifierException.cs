namespace Siderite.Code.Errors
{
    public class StartCannotHaveQuantifierException : ParseException
    {
        public StartCannotHaveQuantifierException() : base("Quantifier after the ^ character.")
        {
        }
    }
}