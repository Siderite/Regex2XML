namespace Siderite.Code.Errors
{
    public class EmptyCharacterClassException : ParseException
    {
        public EmptyCharacterClassException() : base("Empty character class.")
        {
        }
    }
}