namespace Siderite.Code.Errors
{
    public class OrInCharacterClassException : ParseException
    {
        public OrInCharacterClassException() : base("Or (|) character in character class.")
        {
        }
    }
}