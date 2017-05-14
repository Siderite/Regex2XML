namespace Siderite.Code.Errors
{
    public class UnexpectedQuestionMarkException : ParseException
    {
        public UnexpectedQuestionMarkException() : base("Unexpected ? character.")
        {
        }
    }
}