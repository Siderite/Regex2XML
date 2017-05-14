namespace Siderite.Code.Errors
{
    public class InvalidRootNameException : ParseException
    {
        public InvalidRootNameException() : base("Invalid root element name.")
        {
        }
    }
}