using System;

namespace Siderite.Code
{
    public class LookAroundType
    {
        private const string negativeName = "negative";
        private const string positiveName = "positive";
        private const string forwardName = "forward";
        private const string backwardName = "backward";

        public LookAroundType() {}
        public LookAroundType(bool positive, bool forward)
        {
            Positive = positive;
            Forward = forward;
        }

        private bool _negative;
        private bool _backward;
        public bool Positive
        {
            get { return !_negative; }
            set { _negative = !value; }
        }
        public bool Forward
        {
            get { return !_backward; }
            set { _backward = !value; }
        }
        public override string ToString()
        {
            return string.Format("{0},{1}",
                (Positive ? positiveName : negativeName),
                (Forward ? forwardName : backwardName));
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LookAroundType)) return false;
            return Equals((LookAroundType) obj);
        }

        public bool Equals(LookAroundType obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj._negative.Equals(_negative) && obj._backward.Equals(_backward);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_negative.GetHashCode()*397) ^ _backward.GetHashCode();
            }
        }
        public static LookAroundType Parse(string s)
        {
            var splits = (s ?? "").ToLower().Split(',');
            if (splits.Length!=2) throw new ArgumentException("Could not parse '"+s+"' as a LookAroundType");
            var result = new LookAroundType();
            switch(splits[0])
            {
                case positiveName:
                    result.Positive = true;
                    break;
                case negativeName:
                    result.Positive = false;
                    break;
                default:
                    throw new ArgumentException("First part of a LookAroundType must be either "+positiveName+" or "+negativeName);
            }
            switch(splits[1])
            {
                case forwardName:
                    result.Forward = true;
                    break;
                case backwardName:
                    result.Forward = false;
                    break;
                default:
                    throw new ArgumentException("Second part of a LookAroundType must be either "+forwardName+" or "+backwardName);
            }
            return result;
        }

        public string ToRegex()
        {
            return string.Format("?{0}{1}",
                                 (Forward ? "" : "<"),
                                 (Positive ? "=" : "!"));
        }
    }
}
