namespace Siderite.Code.Errors
{
    public class QuantifierCannotHaveQuantifierException : ParseException
    {
        public QuantifierCannotHaveQuantifierException() : base("Quantifier cannot have a quantifier.")
        {
        }
    }
}