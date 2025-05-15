namespace TFLaComp_CW.Parsers
{
    public class Lexer
    {
        public List<Token> GetTokens(string input)
        {
            var lines = input.Split('\n');
            var tokens = new List<Token>();

            int index = 0;

            for (int l = 0; l < lines.Length; l++)
            {
                var parts = lines[l]
                    .Replace("(", " ( ")
                    .Replace(")", " ) ")
                    .Replace(",", " , ")
                    .Replace(";", " ; ")
                    .Split(new[] { ' ', '\t', '\v' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i++, index++)
                {
                    string s = parts[i];
                    var ls = parts[i].ToLower();

                    TokenType type = ls switch
                    {
                        "read" => TokenType.READ,
                        "(" => TokenType.LPAREN,
                        ")" => TokenType.RPAREN,
                        "*" => TokenType.STAR,
                        "," => TokenType.COMMA,
                        ";" => TokenType.SEMICOLON,
                        _ => TokenType.UNKNOWN
                    };

                    if (type == TokenType.UNKNOWN)
                    {
                        if (char.IsLetter(s[0]) == true)
                        {
                            type = TokenType.VARIABLE;
                        }

                        for (int j = 0; j < s.Length; j++)
                        {
                            if (char.IsLetterOrDigit(s[j]) == false)
                            {
                                type = TokenType.UNKNOWN;
                                break;
                            }
                        }
                    }

                    tokens.Add(new Token(type, s, index, l + 1));
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", index, lines.Length));

            return tokens;
        }
    }
}
