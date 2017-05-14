namespace Siderite.Code.Errors
{
    public class MalformedNamedBackreference : ParseException
    {
        public MalformedNamedBackreference() : base("Malformed named backreference (\\k)")
        {
        }
    }
}