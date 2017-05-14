namespace Siderite.Code
{
    /// <summary>
    /// roles of characters in the regular expression pattern
    /// </summary>
    public enum CharType
    {
        None,
        Vocab,
        CharClassStart,
        CharClassEnd,
        GroupStart,
        GroupEnd,
        LineEnd,
        LineStart,
        Or,
        Star,
        Plus,
        GroupModifier,
        AtMostOne,
        LazyModifier,
        NumericQuantifierEnd,
        NumericQuantifierStart,
        NumericQuantifierMin,
        NumericQuantifierMax,
        NumericQuantifierComma,
        WhiteSpaceComment,
        CarriageReturn,
        NewLine,
        NamedCharacterClass,
        NamedBackreference,
        NegatedNamedCharacterClass,
        NegatedNamedBackreference
    }
}