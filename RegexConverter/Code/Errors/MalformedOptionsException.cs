using System;

namespace Siderite.Code.Errors
{
    public class MalformedOptionsException : ParseException
    {
        public MalformedOptionsException(Exception ex) : base("Error parsing the options attribute: " + ex.Message)
        {
        }
    }
}