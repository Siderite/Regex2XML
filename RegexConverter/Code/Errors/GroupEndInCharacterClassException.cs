namespace Siderite.Code.Errors
{
    public class GroupEndInCharacterClassException : ParseException
    {
        public GroupEndInCharacterClassException() : base("Group end ) character in character class.")
        {
        }
    }
}