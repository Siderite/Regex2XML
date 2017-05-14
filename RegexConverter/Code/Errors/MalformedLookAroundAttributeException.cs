using System;

namespace Siderite.Code.Errors
{
    public class MalformedLookAroundAttributeException : ParseException
    {
        public MalformedLookAroundAttributeException(Exception ex)
            : base("Malformed look around attribute: " + ex.Message)
        {
        }
    }
}