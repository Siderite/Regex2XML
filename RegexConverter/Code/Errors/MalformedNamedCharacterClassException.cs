namespace Siderite.Code.Errors
{
    public class MalformedNamedCharacterClassException : ParseException
    {
        public MalformedNamedCharacterClassException() : base("Malformed named character class (\\p)")
        {
        }
    }
}