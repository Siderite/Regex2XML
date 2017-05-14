namespace Siderite.Code.Errors
{
    public class UnsuportedGroupModifierException : ParseException
    {
        public UnsuportedGroupModifierException(string gm) : base("Unsuported group modifier: " + gm)
        {
        }
    }
}