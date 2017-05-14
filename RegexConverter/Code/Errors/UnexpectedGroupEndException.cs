namespace Siderite.Code.Errors
{
    public class UnexpectedGroupEndException : ParseException
    {
        public UnexpectedGroupEndException() : base("Unexpected ')' character.")
        {
        }
    }
}