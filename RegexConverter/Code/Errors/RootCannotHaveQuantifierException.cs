namespace Siderite.Code.Errors
{
    public class RootCannotHaveQuantifierException : ParseException
    {
        public RootCannotHaveQuantifierException() : base("The root element cannot a quantifier.")
        {
        }
    }
}