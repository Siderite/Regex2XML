namespace Siderite.Code.Errors
{
    public class GroupStartInCharacterClassException : ParseException
    {
        public GroupStartInCharacterClassException() : base("Group start ( character in character class.")
        {
        }
    }
}