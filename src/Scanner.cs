using static lox.TokenType;

namespace lox;

public class Scanner
{


    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 0;


    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(EOF, string.Empty, null, line));
        return tokens;
    }

    private bool IsAtEnd() => current >= source.Length;

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break;
            case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
            case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
            case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
            case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
            case ' ':
            case '\r':
            case '\t':
                //ignore whitespace
                break;
            case '"': this.String(); break;
            case '\n':
                line++;
                break;
            case '/':
                if (Match('/'))
                {
                    //comments goto end of line
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    // slash for division
                    AddToken(SLASH);
                }
                break;
            default:

                if (IsDigit(c))
                {
                    Number();
                    break;
                }
                else if (IsAplha(c))
                {
                    Identifier();
                    break;
                }
                else
                {
                    Lox.Error(line, $"Unexpected character '{c}'");
                    break;
                }
        }
    }

    private char Advance() => source[current++];

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object literal)
    {
        string text = source.Substring(start, current);
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (source[current] != expected)
        {
            return false;
        }

        current++;
        return true;
    }

    private char Peek(int length = 1)
    {
        // just to make the arguments more intuitive
        int offset = length - 1;

        if (offset < 0)
        {
            throw new ArgumentException("Peek must be forward");
        }

        if (offset > 1)
        {
            throw new ArgumentException("Max peek is 2");
        }

        if (IsAtEnd())
        {
            return '\0';
        }

        if (current + offset > source.Length)
        {
            return '\0';
        }

        return source[current + offset];
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                line++;
            }

            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string constant");
            return;
        }

        //Move past the closing " since we just peeked before
        Advance();

        //remove the opening and closing "
        string value = source.Substring(start + 1, current - 1);

        AddToken(STRING, value);
    }

    private bool IsDigit(char c) => c >= '0' && c <= '9';
    private bool IsAplha(char c) => 
        c >= 'a' && c <= 'z' ||
        c >= 'A' && c <= 'Z' ||
        c == '_';

    private bool IsAplhaNumeric(char c) => 
        IsAplha(c) ||
        IsDigit(c);

    private void Number()
    {
        void ScanDigits()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        ScanDigits();

        if (Peek() == '.' && IsDigit(Peek(2)))
        {
            Advance();
            ScanDigits();
        }

        AddToken(NUMBER, double.Parse(source.Substring(start, current)));
    }

    private void Identifier()
    {
        while(IsAplhaNumeric(Peek()))
        {
            Advance();
        }

        AddToken(IDENTIFIER, source.Substring(start, current));
    }

}