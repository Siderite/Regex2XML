namespace Siderite.Code.Errors
{
    public class InvalidSettingException : ParseException
    {
        public InvalidSettingException(string setting) : base("Invalid setting: " + setting + ".")
        {
        }
    }
}