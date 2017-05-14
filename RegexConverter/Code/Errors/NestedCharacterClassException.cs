namespace Siderite.Code.Errors
{
    public class NestedCharacterClassException : ParseException
    {
        public NestedCharacterClassException() : base("[ character in character class.")
        {
        }
    }
}