namespace Siderite.Code.Errors
{
    public class UnexpectedNumericQualifierEndException : ParseException
    {
        public UnexpectedNumericQualifierEndException() : base("Unexpected } character.")
        {
        }
    }
}