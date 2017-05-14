namespace Siderite.Code.Errors
{
    public class UnexpectedCharacterClassEndException : ParseException
    {
        public UnexpectedCharacterClassEndException() : base("Unexpected ] character.")
        {
        }
    }
}