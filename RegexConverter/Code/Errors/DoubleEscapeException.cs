namespace Siderite.Code.Errors
{
    public class DoubleEscapeException : ParseException
    {
        public DoubleEscapeException()
            : base("Characters that need escaping in string characters cannot be directly escaped in regex")
        {
        }
    }
}