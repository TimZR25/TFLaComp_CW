namespace TFLaComp_CW.Parsers
{
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public int Position { get; }

        public Token(TokenType type, string lexeme, int pos)
        {
            Type = type;
            Lexeme = lexeme;
            Position = pos;
        }
    }
}

