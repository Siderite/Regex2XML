namespace Siderite.Code.Errors
{
    public class IncompleteNonExpressionCannotHaveQuantifierException : ParseException
    {
        public IncompleteNonExpressionCannotHaveQuantifierException()
            : base("Only expressions can be incomplete and have a quantifier.")
        {
        }
    }
}