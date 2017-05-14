namespace Siderite.Code.Errors
{
    public class InvalidElementNameException : ParseException
    {
        public InvalidElementNameException(string name) : base("Unknown xml element name: " + name + ".")
        {
        }
    }
}