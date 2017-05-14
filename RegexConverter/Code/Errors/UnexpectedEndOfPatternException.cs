namespace Siderite.Code.Errors
{
    public class UnexpectedEndOfPatternException : ParseException
    {
        public UnexpectedEndOfPatternException() : base("Unexpected end of regular epxression.")
        {
        }
    }
}