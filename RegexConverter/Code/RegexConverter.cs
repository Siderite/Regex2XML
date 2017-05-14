using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Siderite.Code.Errors;

namespace Siderite.Code
{
    /// <summary>
    /// Class to convert from .Net Regex format regular 
    /// expressions into XML and back
    /// </summary>
    public class RegexConverter
    {
        private const string schema = "schema";

        // element names.
        private const string charClassName = "class";
        private const string endName = "end";
        private const string expressionName = "expression";
        private const string groupName = "group";
        private const string orName = "or";
        private const string setName = "set";
        private const string rootName = "regex";
        private const string startName = "start";
        private const string commentName = "comment";
        private const string carriageReturnName = "cr";
        private const string newLineName = "nl";
        private const string namedBackrefenceName = "namedBackref";
        private const string namedCharacterClassName = "namedClass";


        // attributes used in the XML
        private const string indexAttr = "index";
        private const string nameAttr = "name";
        private const string quantifierAttr = "quantifier";
        private const string xmodeAttr = "xmode";
        private const string conditionalAttr = "conditional";
        private const string theConditionAttr = "condition";
        private const string nonGroupingAttr = "nonGrouping";
        private const string negatedAttr = "negated";
        private const string optionsAttr = "options";
        private const string originalAttr = "original";
        private const string lookAroundAttr = "lookAround";
        private const string atomicAttr = "atomic";
        private const string nameCharAttr = "nameChar";

        // the regex pattern
        private readonly string _regex;
        // if the last element has completed 
        //(group ended or character class ended)
        private bool _elementComplete;
        // index of the current unnamed grouping group
        private int _groupIndex;
        // position in the regex pattern
        private int _index;
        // currently working on node
        private XElement _lastNode;
        // current Regex options
        private RegexOptions _options;
        // stack of group settings
        private Stack<RegexOptions> _optionsStack;
        // quantifier step: 0{1,2}3?4
        private QuantifierState _quantifierState;
        // if anything is escaped
        private bool _slash;
        // the generated XDocument
        private XDocument _xdoc;
        // stack for dynamically changed options


        /// <summary>
        /// takes a regular expression pattern
        /// and parses it into a XDocument
        /// </summary>
        /// <param name="pattern"></param>
        public RegexConverter(string pattern)
            : this(new Regex(pattern))
        {
        }

        /// <summary>
        /// takes a Regex objects
        /// and parses it into a XDocument
        /// </summary>
        /// <param name="regex"></param>
        public RegexConverter(Regex regex)
        {
            _regex = regex.ToString();
            _options = regex.Options;
            Parse();
        }

        // stack of regex options (for groups with settings)
        private Stack<RegexOptions> OptionsStack
        {
            get
            {
                if (_optionsStack == null) _optionsStack = new Stack<RegexOptions>();
                return _optionsStack;
            }
        }

        /// <summary>
        /// The XDocument resulted from the
        /// regular expression parse
        /// </summary>
        public XDocument Doc
        {
            get { return _xdoc; }
        }

        // is IgnoreWhiteSpace set?
        private bool IsIgnoreWhiteSpace()
        {
            return IsOption(_options, RegexOptions.IgnorePatternWhitespace);
        }

        // is ExplicitCapture set?
        private bool IsExplicitCapture()
        {
            return IsOption(_options, RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// The actual parsing.
        /// </summary>
        private void Parse()
        {
            _index = 0;
            _slash = false;
            _groupIndex = 0;
            // the root of the XDocument
            var root = new XElement(rootName);
            // save regex options
            root.SetAttributeValue(optionsAttr, _options.ToString());
            _xdoc = new XDocument(root);
            _lastNode = _xdoc.Root;
            _elementComplete = true;
            while (_index < _regex.Length)
            {
                Process();
            }
            // clean up XDocument (removing useless expressions)
            CleanUp();
        }

        /// <summary>
        /// Clear empty expressions and set content to groups with only one simple expression
        /// </summary>
        private void CleanUp()
        {
            // if any expression has no content, remove it
            List<XElement> list = (from n in _xdoc.Descendants(expressionName)
                                   select n).ToList();
            foreach (XElement element in list)
            {
                if (String.IsNullOrEmpty(GetContent(element)))
                {
                    element.Remove();
                }
            }
            IEnumerable<XElement> groups = from n in _xdoc.Descendants(groupName)
                                           select n;
            // if any group has only one child expression with no quantifiers, 
            // set its content to the expression content and remove the expression
            foreach (XElement g in groups)
            {
                if (g.Nodes().Count() != 1) continue;

                var e = g.FirstNode as XElement;
                if (e != null && e.Name == expressionName && String.IsNullOrEmpty(GetQuantifier(e)))
                {
                    e.Remove();
                    SetContent(g, GetContent(e));
                }
            }
        }

        /// <summary>
        /// Process current character in regular expression
        /// </summary>
        private void Process()
        {
            char ch = _regex[_index];
            // Do things based on the current character type
            CharType type = GetCharType(ch);
            switch (type)
            {
                default:
                    // ReSharper disable RedundantCaseLabel
                case CharType.Vocab:
                    // ReSharper restore RedundantCaseLabel
                    {
                        // if last element is closed, create new expression
                        // and you cannot add conetnt directly to a group
                        if (_elementComplete || IsGroup(_lastNode))
                            NewExpression();
                        // only an expression or a character class at this time
                        AddContent(_lastNode, ch.ToString());
                    }
                    break;
                case CharType.CarriageReturn:
                    {
                        NewCarriageReturn();
                    }
                    break;
                case CharType.NewLine:
                    {
                        NewNewLine();
                    }
                    break;
                case CharType.CharClassStart:
                    {
                        // add new character class
                        NewCharClass();
                    }
                    break;
                case CharType.CharClassEnd:
                    {
                        // complete character class
                        _elementComplete = true;
                    }
                    break;
                case CharType.GroupStart:
                    {
                        // create a new group
                        NewGroup();
                    }
                    break;
                case CharType.GroupEnd:
                    {
                        // complete current group
                        _lastNode = LastParent();
                        _elementComplete = true;
                        if (!string.IsNullOrEmpty(GetAttributeValue(_lastNode, optionsAttr)))
                            _options = OptionsStack.Pop();
                    }
                    break;
                case CharType.LineEnd:
                    {
                        // create an End element already completed
                        NewEnd();
                        _elementComplete = true;
                    }
                    break;
                case CharType.LineStart:
                    {
                        // create an Start element already completed
                        NewStart();
                        _elementComplete = true;
                    }
                    break;
                case CharType.Or:
                    {
                        // create an Or element already completed
                        NewOr();
                        _elementComplete = true;
                    }
                    break;
                case CharType.Star:
                    {
                        // set the quantifier
                        ApplyQuantifier("*");
                    }
                    break;
                case CharType.Plus:
                    {
                        // set the quantifier
                        ApplyQuantifier("+");
                    }
                    break;
                case CharType.AtMostOne:
                    {
                        // set the quantifier
                        ApplyQuantifier("?");
                    }
                    break;
                case CharType.LazyModifier:
                    {
                        // add the lazy operator to the quantifier
                        AddQuantifier(_lastNode, "?");
                    }
                    break;
                case CharType.NumericQuantifierStart:
                    {
                        // set the first curly bracket as a new quantifier
                        ApplyQuantifier("{");
                    }
                    break;
                case CharType.NumericQuantifierMin:
                case CharType.NumericQuantifierComma:
                case CharType.NumericQuantifierMax:
                case CharType.NumericQuantifierEnd:
                    {
                        // add the character to the numeric range quantifier
                        AddQuantifier(_lastNode, ch.ToString());
                    }
                    break;
                case CharType.GroupModifier:
                    {
                        // process group modifiers
                        char modifier = _regex[_index + 1];
                        switch (modifier)
                        {
                            case ':': // non grouping
                                RemoveGroupIndex();
                                _lastNode.SetAttributeValue(nonGroupingAttr, true);
                                _index ++;
                                break;
                            case '<':
                                _index += 2;
                                switch (_regex[_index])
                                {
                                    case '=': // positive lookbehind
                                        {
                                            RemoveGroupIndex();
                                            SetLookAround(_lastNode, true, false);
                                        }
                                        break;
                                    case '!': // negative lookbehind
                                        {
                                            RemoveGroupIndex();
                                            SetLookAround(_lastNode, false, false);
                                        }
                                        break;
                                    default: //name or balancing group
                                        string name = "";
                                        if (_index >= _regex.Length) throw new UnexpectedEndOfPatternException();
                                        while (_regex[_index] != '>')
                                        {
                                            name += _regex[_index];
                                            _index++;
                                        }
                                        RemoveGroupIndex();
                                        SetName(_lastNode, name);
                                        if (modifier == '\'') _lastNode.SetAttributeValue(nameCharAttr, modifier);
                                        break;
                                }
                                break;
                            case '\'': // undocumented quote group naming
                                {
                                    _index += 2;

                                    string name = "";
                                    if (_index >= _regex.Length) throw new UnexpectedEndOfPatternException();
                                    while (_regex[_index] != '\'')
                                    {
                                        name += _regex[_index];
                                        _index++;
                                    }
                                    RemoveGroupIndex();
                                    SetName(_lastNode, name);
                                    if (modifier == '\'') _lastNode.SetAttributeValue(nameCharAttr, modifier);
                                }
                                break;
                            case 'i': //options
                            case 'm':
                            case 'n':
                            case 's':
                            case 'x':
                            case 'I':
                            case 'M':
                            case 'N':
                            case 'S':
                            case 'X':
                            case '-':
                                bool set = true;
                                _index++;
                                bool inOptions = true;
                                string original = "";
                                OptionsStack.Push(_options);
                                while (inOptions)
                                {
                                    if (_index >= _regex.Length) throw new UnexpectedEndOfPatternException();
                                    char option = _regex[_index];
                                    switch (option)
                                    {
                                        case '-': // unset options
                                            set = false;
                                            original += option;
                                            break;
                                        case 'i':
                                        case 'I':
                                            SetOption(RegexOptions.IgnoreCase, set);
                                            original += option;
                                            break;
                                        case 'm':
                                        case 'M':
                                            SetOption(RegexOptions.Multiline, set);
                                            original += option;
                                            break;
                                        case 'n':
                                        case 'N':
                                            SetOption(RegexOptions.ExplicitCapture, set);
                                            original += option;
                                            break;
                                        case 's':
                                        case 'S':
                                            SetOption(RegexOptions.Singleline, set);
                                            original += option;
                                            break;
                                        case 'x':
                                        case 'X':
                                            SetOption(RegexOptions.IgnorePatternWhitespace, set);
                                            original += option;
                                            break;
                                        case ' ':
                                            //TODO: what does space do? tests with Regex showed them valid, but non functional!
                                            original += option;
                                            break;
                                        case ':': //it's a group with its own settings
                                            inOptions = false;
                                            _lastNode.SetAttributeValue(optionsAttr, _options.ToString());
                                            _lastNode.SetAttributeValue(originalAttr, original);
                                            break;
                                        case ')': // it's a set group. From now on these settings apply.
                                            inOptions = false;
                                            RemoveGroupIndex();
                                            TurnToSet(_lastNode);
                                            _lastNode.SetAttributeValue(optionsAttr, _options.ToString());
                                            _lastNode.SetAttributeValue(originalAttr, original);
                                            OptionsStack.Pop();
                                            break;
                                        default:
                                            throw new UnsuportedGroupModifierException(option.ToString());
                                    }
                                    switch (option)
                                    {
                                        case ')':
                                        case ':':
                                            break;
                                        default:
                                            _index++;
                                            break;
                                    }
                                }
                                break;
                            case '#': // comment
                                {
                                    RemoveGroupIndex();
                                    TurnToComment(_lastNode);
                                    _index += 2;
                                    while (_index < _regex.Length && _regex[_index] != ')')
                                    {
                                        AddContent(_lastNode, _regex[_index].ToString());
                                        _index++;
                                    }
                                    _elementComplete = true;
                                }
                                break;
                            case '=': // positive lookaround
                                {
                                    _index ++;
                                    RemoveGroupIndex();
                                    SetLookAround(_lastNode, true, true);
                                }
                                break;
                            case '!': //negative lookaround
                                {
                                    _index ++;
                                    RemoveGroupIndex();
                                    SetLookAround(_lastNode, false, true);
                                }
                                break;
                            case '>': // atomic group
                                {
                                    RemoveGroupIndex();
                                    _lastNode.SetAttributeValue(atomicAttr, true);
                                    _index ++;
                                }
                                break;
                            case '(': // conditional group
                                {
                                    RemoveGroupIndex();
                                    _index ++;
                                    _lastNode.SetAttributeValue(conditionalAttr, true);
                                    NewGroup();
                                    SetIndex(_lastNode, 0);// remove index but don't decrease index
                                    _lastNode.SetAttributeValue(theConditionAttr, true);
                                }
                                break;
                            default:
                                throw new UnsuportedGroupModifierException("?" + _regex[_index + 1]);
                        }
                    }
                    break;
                case CharType.WhiteSpaceComment:
                    {
                        NewComment();
                        _lastNode.SetAttributeValue(xmodeAttr, true);
                        _index++;
                        while (_index < _regex.Length)
                        {
                            if (_regex[_index] == '\r' || _regex[_index] == '\n')
                            {
                                _index--;
                                break;
                            }
                            AddContent(_lastNode, _regex[_index].ToString());
                            _index++;
                        }
                        _elementComplete = true;
                    }
                    break;
                case CharType.NamedCharacterClass:
                case CharType.NegatedNamedCharacterClass:
                    {
                        string content = GetContent(_lastNode);
                        SetContent(_lastNode, content.Substring(0, content.Length - 1));
                        NewNamedCharacterClass();
                        if (type == CharType.NegatedNamedCharacterClass)
                            _lastNode.SetAttributeValue(negatedAttr, true);
                        _index += 2;
                        while (_index < _regex.Length && _regex[_index] != '}')
                        {
                            AddContent(_lastNode, _regex[_index].ToString());
                            _index++;
                        }
                        _elementComplete = true;
                    }
                    break;
                case CharType.NamedBackreference:
                case CharType.NegatedNamedBackreference:
                    {
                        string content = GetContent(_lastNode);
                        SetContent(_lastNode, content.Substring(0, content.Length - 1));
                        NewNamedBackreference();
                        if (type == CharType.NegatedNamedBackreference)
                            _lastNode.SetAttributeValue(negatedAttr, true);
                        _index += 2;
                        while (_index < _regex.Length && _regex[_index] != '>')
                        {
                            AddContent(_lastNode, _regex[_index].ToString());
                            _index++;
                        }
                        _elementComplete = true;
                    }
                    break;
            }
            _index++;
        }

        // set lookaround attribute
        private static void SetLookAround(XElement node, bool positive, bool forward)
        {
            node.SetAttributeValue(lookAroundAttr, new LookAroundType(positive, forward));
        }

        // remove group index and decrease the groupIndex value
        // practically undo the reverse operation at the start of a group
        private void RemoveGroupIndex()
        {
            _groupIndex--;
            SetIndex(_lastNode, 0);
        }

        // Set or unset a regex option in a set of options flags
        private static void SetOption(ref RegexOptions flags, RegexOptions options, bool set)
        {
            if (set)
            {
                flags |= options;
            }
            else
            {
                flags &= ~options;
            }
        }

        // set or unset a regex option in the current parsing options
        private void SetOption(RegexOptions options, bool set)
        {
            SetOption(ref _options, options, set);
        }

        // add new named backreference (\k<name>)
        private void NewNamedBackreference()
        {
            NewElement(namedBackrefenceName);
        }

        // add new named character class (\p{name})
        private void NewNamedCharacterClass()
        {
            NewElement(namedCharacterClassName);
        }

        // create a new comment element
        private void NewComment()
        {
            NewElement(commentName);
        }

        // turn group to set
        private void TurnToSet(XElement element)
        {
            element.Name = setName;
            _elementComplete = true;
        }

        // turn group to comment
        private void TurnToComment(XElement element)
        {
            element.Name = commentName;
            element.SetAttributeValue(nameAttr, null);
            _elementComplete = true;
        }

        // this is for primary quantifiers chars (not lazy)
        private void ApplyQuantifier(string quantifier)
        {
            // if the quantifier applies to an uncompleted element
            // it means it applies to a single character
            // create a separate expression from it
            if (!_elementComplete)
            {
                string content = GetContent(_lastNode);
                // the character might be escaped
                int charLength = 1;
                int i = content.Length - 2;
                while (i >= 0)
                {
                    if (content[i] == '\\') charLength = 3 - charLength;
                    else break;
                    i--;
                }
                string charString = content.Substring(content.Length - charLength, charLength);
                SetContent(_lastNode, content.Substring(0, content.Length - charLength));
                // create the new expression
                NewExpression();
                SetContent(_lastNode, charString);
                _elementComplete = true;
            }
            // set the quantifier for the current element
            SetQuantifier(_lastNode, quantifier);
        }

        // add a quantifier character (for numeric or lazy quantifiers)
        private static void AddQuantifier(XElement element, string s)
        {
            XAttribute attr = element.Attribute(quantifierAttr);
            if (attr == null) SetQuantifier(element, s);
            else attr.Value += s;
        }

        // set a quantifier value
        private static void SetQuantifier(XElement element, string s)
        {
            element.SetAttributeValue(quantifierAttr, s);
        }

        // create a new carriage return element (|)
        private void NewCarriageReturn()
        {
            NewElement(carriageReturnName);
            _elementComplete = true;
        }

        // create a new Or element (|)
        private void NewNewLine()
        {
            NewElement(newLineName);
            _elementComplete = true;
        }


        // create a new Or element (|)
        private void NewOr()
        {
            NewElement(orName);
            _elementComplete = true;
        }

        // create a new Start element (^)
        private void NewStart()
        {
            NewElement(startName);
            _elementComplete = true;
        }

        // create a new End element ($)
        private void NewEnd()
        {
            NewElement(endName);
            _elementComplete = true;
        }

        // create a new Group (curvy brackets)
        private void NewGroup()
        {
            NewElement(groupName);
            _groupIndex++;
            if (!IsExplicitCapture())
                SetIndex(_lastNode, _groupIndex);
        }

        // set the index attribute for groups
        private static void SetIndex(XElement element, int index)
        {
            element.SetAttributeValue(indexAttr, index == 0 ? (object) null : index);
        }

        // set the name of the element (only for groups)
        private static void SetName(XElement element, string name)
        {
            element.SetAttributeValue(nameAttr, name);
        }

        // create a new character class (square brackets)
        private void NewCharClass()
        {
            NewElement(charClassName);
        }

        // find the parent of the current element
        // if the element is a group, then the parent is itself
        // or the next group if this element is completed
        // otherwise it is the parent group of the current element
        // it can also be the root
        private XElement LastParent()
        {
            XElement node = _lastNode;
            if (_elementComplete && IsGroup(node)) node = node.Parent;
            while (node != null && !IsGroup(node) && !IsRoot(node))
                node = node.Parent;
            return node;
        }

        // check the name of the element against a string
        private static bool Is(XElement node, string name)
        {
            return string.Equals(node.Name.ToString(), name, StringComparison.CurrentCultureIgnoreCase);
        }

        // is the element root?
        private static bool IsRoot(XElement node)
        {
            return Is(node, rootName);
        }

        // is the element group?
        private static bool IsGroup(XElement node)
        {
            return Is(node, groupName);
        }

        // add a string to the element's content
        private static void AddContent(XElement element, string s)
        {
            element.Value += s;
            return;
        }

        // set the content of an element (expressions)
        private static void SetContent(XElement element, string s)
        {
            element.Value = s;
        }

        // create a new element with a certain name
        // set uncompleted and as current node
        private void NewElement(string name)
        {
            var e = new XElement(name);
            LastParent().Add(e);
            _lastNode = e;
            _elementComplete = false;
        }

        // create a new expression
        private void NewExpression()
        {
            NewElement(expressionName);
        }

        // detect the type of the current char
        private CharType GetCharType(char ch)
        {
            // it is escaped
            if (_slash)
            {
                _slash = false;
                switch (ch)
                {
                        //characters that need escaping
                    case '\a': //  Alert (bell)  
                    case '\b': //  Backspace  
                    case '\f': //  Formfeed  
                    case '\n': //  Newline  
                    case '\r': //  Carriage return  
                    case '\t': //  Horizontal tab  
                    case '\v': //  Vertical tab  
                    case '\0': //  Null character  
                        throw new DoubleEscapeException();
                    case 'p': // named character classes
                        if (_index + 1 >= _regex.Length) throw new UnexpectedEndOfPatternException();
                        if (_regex[_index + 1] != '{') throw new MalformedNamedCharacterClassException();
                        return CharType.NamedCharacterClass;
                    case 'P':
                        if (_index + 1 >= _regex.Length) throw new UnexpectedEndOfPatternException();
                        if (_regex[_index + 1] != '{') throw new MalformedNamedCharacterClassException();
                        return CharType.NegatedNamedCharacterClass;
                    case 'k': // named character classes
                        if (_index + 1 >= _regex.Length) throw new UnexpectedEndOfPatternException();
                        if (_regex[_index + 1] != '<') throw new MalformedNamedBackreference();
                        return CharType.NamedBackreference;
                    case 'K':
                        if (_index + 1 >= _regex.Length) throw new UnexpectedEndOfPatternException();
                        if (_regex[_index + 1] != '<') throw new MalformedNamedBackreference();
                        return CharType.NegatedNamedBackreference;
                    default:
                        return CharType.Vocab;
                }
            }
            switch (ch)
            {
                default: // normal character, check for no numeric quantifiers
                    CheckNoQuantifier();
                    return CharType.Vocab;
                case '\\': // escape character
                    CheckNoQuantifier();
                    _slash = true;
                    return CharType.Vocab;
                case '[': // character class begin
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) throw new NestedCharacterClassException();
                    return CharType.CharClassStart;
                case ']': // character class end
                    CheckNoQuantifier();
                    if (!IsCharClass(_lastNode) || _elementComplete) throw new UnexpectedCharacterClassEndException();
                    if (String.IsNullOrEmpty(GetContent(_lastNode))) throw new EmptyCharacterClassException();
                    return CharType.CharClassEnd;
                case '(': //  group begin
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    return CharType.GroupStart;
                case ')': // group end
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    if (!IsGroup(LastParent())) throw new UnexpectedGroupEndException();
                    return CharType.GroupEnd;
                case '$': // end of line
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    return CharType.LineEnd;
                case '^': // can be either start of line or the not operator
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    return CharType.LineStart;
                case '|': // or operator
                    CheckNoQuantifier();
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    return CharType.Or;
                case '*': // zero or more operator
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    CheckQuantifier();
                    return CharType.Star;
                case '+': // one or more operator
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    CheckQuantifier();
                    return CharType.Plus;
                case '?': // zero or one operator, lazy modifier or group modifier
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    if (IsGroup(_lastNode) && !_lastNode.HasElements)
                    {
                        if (_index + 1 >= _regex.Length) throw new UnexpectedEndOfPatternException();
                        return CharType.GroupModifier;
                    }
                    switch (_quantifierState)
                    {
                        case QuantifierState.None:
                            CheckQuantifier();
                            _quantifierState = QuantifierState.PreLazy;
                            return CharType.AtMostOne;
                        case QuantifierState.PreLazy:
                            _quantifierState = QuantifierState.End;
                            return CharType.LazyModifier;
                        default:
                            throw new UnexpectedQuestionMarkException();
                    }
                case '}': // end of numeric range quantifier
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    switch (_quantifierState)
                    {
                        case QuantifierState.NumericMinValue:
                        case QuantifierState.NumericMaxValue:
                            break;
                        default:
                            throw new UnexpectedNumericQualifierEndException();
                    }
                    _quantifierState = QuantifierState.PreLazy;
                    return CharType.NumericQuantifierEnd;
                case '{': // start of numeric range quantifier
                    if (IsCharClass(_lastNode) && !_elementComplete) return CharType.Vocab;
                    CheckQuantifier();
                    _quantifierState = QuantifierState.NumericMinValue;
                    return CharType.NumericQuantifierStart;
                case '0': // digits have meaning only in the numeric range quantifier
                case '1': // otherwise they are just normal characters
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    switch (_quantifierState)
                    {
                        case QuantifierState.NumericMinValue:
                            return CharType.NumericQuantifierMin;
                        case QuantifierState.NumericMaxValue:
                            return CharType.NumericQuantifierMax;
                    }
                    return CharType.Vocab;
                case ',': // comma has meaning only in a numeric quantifier
                    if (_quantifierState == QuantifierState.NumericMinValue)
                    {
                        _quantifierState = QuantifierState.NumericMaxValue;
                        return CharType.NumericQuantifierComma;
                    }
                    return CharType.Vocab;
                case '#': // hash has meaning only if ignore whitespace is set
                    if (IsIgnoreWhiteSpace()&&(!IsCharClass(_lastNode)||_elementComplete)) return CharType.WhiteSpaceComment;
                    return CharType.Vocab;
                case '\r':
                    CheckNoQuantifier();
                    return CharType.CarriageReturn;
                case '\n':
                    CheckNoQuantifier();
                    return CharType.NewLine;
            }
        }

        // check if you are not in a numeric range quantifier
        private void CheckNoQuantifier()
        {
            switch (_quantifierState)
            {
                case QuantifierState.NumericMinValue:
                case QuantifierState.NumericMaxValue:
                    throw new InvalidNumericQuantifierException();
            }
            _quantifierState = QuantifierState.None;
        }

        // check if a quantifier can be applied
        private void CheckQuantifier()
        {
            if (_elementComplete)
            {
                if (IsStart(_lastNode)) throw new StartCannotHaveQuantifierException();
                if (IsEnd(_lastNode)) throw new EndCannotHaveQuantifierException();
                if (IsOr(_lastNode)) throw new OrCannotHaveQuantifierException();
                if (IsRoot(_lastNode)) throw new RootCannotHaveQuantifierException();
            }
            else
            {
                if (!IsExpression(_lastNode)) throw new IncompleteNonExpressionCannotHaveQuantifierException();
            }
            switch (_quantifierState)
            {
                case QuantifierState.PreLazy:
                case QuantifierState.End:
                    throw new QuantifierCannotHaveQuantifierException();
            }
            _quantifierState = QuantifierState.PreLazy;
        }

        // is it an Or element?
        private static bool IsOr(XElement element)
        {
            return Is(element, orName);
        }

        // is it an End element?
        private static bool IsEnd(XElement element)
        {
            return Is(element, endName);
        }

        // is it a Start element?
        private static bool IsStart(XElement element)
        {
            return Is(element, startName);
        }

        // is it an Expression element?
        private static bool IsExpression(XElement element)
        {
            return Is(element, expressionName);
        }

        // is it a character class element?
        private static bool IsCharClass(XElement element)
        {
            return Is(element, charClassName);
        }

        /// <summary>
        /// this method accepts an XML created by this RegexConverter
        /// and transforms it back into a Regex object
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Regex GetRegex(string xml)
        {
            XDocument doc = XDocument.Load(new StringReader(xml), LoadOptions.PreserveWhitespace);
            if (doc.Root == null || doc.Root.Name.LocalName != rootName)
                throw new InvalidRootNameException();

            string optionsAttribute = GetAttributeValue(doc.Root, optionsAttr);
            RegexOptions options = ParseOptions(optionsAttribute);

            var sb = new StringBuilder();
            var optionsStack = new Stack<RegexOptions>();
            AddRegex(doc.Root, sb, ref options, optionsStack);
            return new Regex(sb.ToString(), options);
        }

        // Is a regex option set in a set of flags?
        private static bool IsOption(RegexOptions flags, RegexOptions option)
        {
            return (flags & option) == option;
        }

        /// <summary>
        /// Parse XML element
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="sb">The StringBuilder where the regular expression is built</param>
        /// <param name="options">current regex options</param>
        /// <param name="optionsStack">the options stack (for groups with settings) </param>
        private static void AddRegex(XElement element, StringBuilder sb, ref RegexOptions options,
                                     Stack<RegexOptions> optionsStack)
        {
            switch (element.Name.LocalName)
            {
                case schema: // ignore schema
                    { }
                    break;
                case startName: // start of line
                    {
                        sb.Append("^");
                    }
                    break;
                case endName: // end of line
                    {
                        sb.Append("$");
                    }
                    break;
                case orName: // or clause
                    {
                        sb.Append("|");
                    }
                    break;
                case rootName: // root of the regex
                    {
                        foreach (XElement e in element.Elements())
                        {
                            AddRegex(e, sb, ref options, optionsStack);
                        }
                    }
                    break;
                case groupName: // group
                    {
                        string content = GetContent(element);
                        string name = GetName(element);
                        string nameChar = GetAttributeValue(element, nameCharAttr) ?? "<";
                        string quantifier = GetQuantifier(element);
                        bool conditional;
                        if (!bool.TryParse(GetAttributeValue(element, conditionalAttr), out conditional))
                            conditional = false;
                        string original = GetAttributeValue(element, originalAttr);
                        string optionsAttribute = GetAttributeValue(element, optionsAttr);
                        bool nonGrouping;
                        if (!bool.TryParse(GetAttributeValue(element, nonGroupingAttr), out nonGrouping))
                            nonGrouping = false;
                        string lookAroundAttribute = GetAttributeValue(element, lookAroundAttr);
                        bool atomic;
                        if (!bool.TryParse(GetAttributeValue(element, atomicAttr), out atomic))
                            atomic = false;
                        if (!string.IsNullOrEmpty(optionsAttribute))
                        {
                            optionsStack.Push(options);
                            options = ParseOptions(optionsAttribute);
                        }
                        sb.Append("(");
                        if (nonGrouping)
                            sb.Append("?:");
                        bool lookAround = !string.IsNullOrEmpty(lookAroundAttribute);
                        if (lookAround)
                        {
                            try
                            {
                                LookAroundType lat = LookAroundType.Parse(lookAroundAttribute);
                                sb.Append(lat.ToRegex());
                            }
                            catch (Exception ex)
                            {
                                throw new MalformedLookAroundAttributeException(ex);
                            }
                        }
                        if (atomic)
                        {
                            sb.Append("?>");
                        }
                        if (!string.IsNullOrEmpty(optionsAttribute))
                        {
                            sb.AppendFormat("?{0}:", original);
                        }
                        if (conditional)
                        {
                            sb.Append("?");
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            char endChar = (nameChar == "\'") ? '\'' : '>';
                            sb.AppendFormat("?{0}{1}{2}", nameChar, name, endChar);
                        }
                        if (element.HasElements)
                        {
                            foreach (var e in element.Elements())
                            {
                                AddRegex(e, sb, ref options, optionsStack);
                            }
                        }
                        else
                        {
                            sb.Append(content);
                        }
                        sb.Append(")");
                        if (!string.IsNullOrEmpty(optionsAttribute))
                        {
                            options = optionsStack.Pop();
                        }
                        if (!string.IsNullOrEmpty(quantifier)) sb.Append(quantifier);
                    }
                    break;
                case expressionName: // normal expression
                    {
                        string content = GetContent(element);
                        string quantifier = GetQuantifier(element);
                        sb.Append(content);
                        if (!string.IsNullOrEmpty(quantifier)) sb.Append(quantifier);
                    }
                    break;
                case charClassName: // character class
                    {
                        string content = GetContent(element);
                        string quantifier = GetQuantifier(element);
                        sb.AppendFormat("[{0}]", content);
                        if (!string.IsNullOrEmpty(quantifier)) sb.Append(quantifier);
                    }
                    break;
                case setName: // set regex options
                    {
                        string original = GetAttributeValue(element, originalAttr);
                        string optionsAttribute = GetAttributeValue(element, optionsAttr);
                        options = ParseOptions(optionsAttribute);
                        sb.AppendFormat("(?{0})", original);
                    }
                    break;
                case commentName: // comment (whitespace or normal)
                    {
                        bool xmode;
                        if (!bool.TryParse(GetAttributeValue(element, xmodeAttr), out xmode)) xmode = false;
                        if (xmode)
                        {
                            sb.AppendFormat("#{0}", GetContent(element));
                        }
                        else
                        {
                            sb.AppendFormat("(?#{0})", GetContent(element));
                        }
                    }
                    break;
                case carriageReturnName: // carriage return (because there is no way to preserve it in XML)
                    {
                        sb.Append("\r");
                    }
                    break;
                case newLineName: // new line character (because there is no way to preserve it in XML)
                    {
                        sb.Append("\n");
                    }
                    break;
                case namedBackrefenceName: // named backreference 
                    {
                        string quantifier = GetQuantifier(element);
                        bool negated;
                        if (!bool.TryParse(GetAttributeValue(element, negatedAttr), out negated)) negated = false;
                        sb.AppendFormat("\\{0}<{1}>",
                                        negated ? "K" : "k",
                                        GetContent(element));
                        if (!string.IsNullOrEmpty(quantifier)) sb.Append(quantifier);
                    }
                    break;
                case namedCharacterClassName: // named character class
                    {
                        string quantifier = GetQuantifier(element);
                        bool negated;
                        if (!bool.TryParse(GetAttributeValue(element, negatedAttr), out negated)) negated = false;
                        sb.AppendFormat("\\{0}{{{1}}}",
                                        negated ? "P" : "p",
                                        GetContent(element));
                        if (!string.IsNullOrEmpty(quantifier)) sb.Append(quantifier);
                    }
                    break;
                default:
                    throw new InvalidElementNameException(element.Name.ToString());
            }
        }

        /// <summary>
        /// Convert string to RegexOptions
        /// </summary>
        /// <param name="optionsAttribute"></param>
        /// <returns></returns>
        private static RegexOptions ParseOptions(string optionsAttribute)
        {
            RegexOptions options;
            try
            {
                options = (RegexOptions) Enum.Parse(typeof (RegexOptions), optionsAttribute, true);
            }
            catch (Exception ex)
            {
                throw new MalformedOptionsException(ex);
            }
            return options;
        }

        // get name attribute value of element (groups only)
        // numeric if unnamed, empty if ungrouping, string if named
        private static string GetName(XElement element)
        {
            return GetAttributeValue(element, nameAttr);
        }

        // get quantifier value
        private static string GetQuantifier(XElement element)
        {
            return GetAttributeValue(element, quantifierAttr);
        }

        // get content value
        private static string GetContent(XElement element)
        {
            return element.Value;
        }

        // generic attribute value method
        private static string GetAttributeValue(XElement element, string name)
        {
            var attr = element.Attribute(name);
            return attr == null ? null : attr.Value;
        }

        public string ApplyXslt(string xsltContent)
        {
            return ApplyXslt(_xdoc, xsltContent);
        }

        public static string ApplyXslt(string xmlContent,string xsltContent)
        {
            var doc = XDocument.Load(new StringReader(xmlContent), LoadOptions.PreserveWhitespace);
            return ApplyXslt(doc, xsltContent);
        }

        /// <summary>
        /// Apply an XSLT transform to an  XDocument and return the result
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xsltContent"></param>
        /// <returns></returns>
        public static string ApplyXslt(XDocument doc , string xsltContent)
        {
            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(new StringReader(xsltContent)));
            var xal = new XsltArgumentList();
            string pattern;
            
            using (var sw = new StringWriter())
            {
                using (var xr = doc.CreateReader())
                {
                    xslt.Transform(xr, xal, sw);
                    pattern = sw.ToString();
                }
            }
            return pattern;
        }

        /// <summary>
        /// Get the regular expression in XML format with or without the schema
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string GetXmlString(XmlMode mode)
        {
            switch (mode)
            {
                case XmlMode.XmlOnly:
                    return Doc.ToString();
                case XmlMode.SchemaOnly:
                    {
                        return GetXmlSchemaString();
                    }
                case XmlMode.XmlWithEmbeddedSchema:
                    {
                        var s = Doc.ToString();
                        s=Regex.Replace(s, @"(\<regex[^>]*?\>)", "${1}\r\n" + GetSchemaXElement());
                        return s;
                    }
                case XmlMode.XmlWithExternalSchema:
                    {
                        var s = Doc.ToString();
                        s = Regex.Replace(s, @"(\<regex[^>]*?)\>", "${1} xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\". XmlRegex.xsd\">");
                        return s;
                    }
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        private static XElement GetSchemaXElement()
        {
            return XElement.Parse(GetXmlSchemaString());
        }

        /// <summary>
        /// Returns the content of the XSLT file necessary to turn most of the RegexXml into a regular expression.
        /// </summary>
        /// <returns></returns>
        public static string GetXml2RegexXsltString()
        {
            using (var st = typeof(RegexConverter).Assembly.GetManifestResourceStream("Siderite.Resources.XmlRegex.xslt"))
            {
                using (var sr=new StreamReader(st, Encoding.Default, true))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Returns the content of the XmlSchema that validates the regex Xml.
        /// </summary>
        /// <returns></returns>
        public static string GetXmlSchemaString()
        {
            using (var st = typeof(RegexConverter).Assembly.GetManifestResourceStream("Siderite.Resources.XmlRegex.xsd"))
            {
                using (var sr = new StreamReader(st, Encoding.Default, true))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}