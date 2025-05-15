namespace TFLaComp_CW.Parsers
{
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public int Position { get; }
        public int Line { get; }

        public Token(TokenType type, string lexeme, int pos, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Position = pos;
            Line = line;
        }
    }
}

