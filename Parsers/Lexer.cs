using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFLaComp_CW.Parsers
{
    public class Lexer
    {
        public List<Token> GetTokens(string input)
        {
            var tokens = new List<Token>();
            var parts = input
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace(",", " , ")
                .Replace(";", " ; ")
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int index = 0;

            for (int i = 0; i < parts.Length; i++, index++)
            {
                var s = parts[i].ToLower();

                TokenType type = s switch
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
                    for (int j = 0; j < s.Length; j++)
                    {
                        if (char.IsLetter(s[j]))
                        {
                            tokens.Add(new Token(TokenType.LETTER, s[j].ToString(), index + j));
                        }
                        else if (char.IsDigit(s[j]))
                        {
                            tokens.Add(new Token(TokenType.DIGIT, s[j].ToString(), index + j));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.UNKNOWN, s[j].ToString(), index + j));
                        }
                    }

                    index += s.Length - 1;

                    continue;
                }

                tokens.Add(new Token(type, s, index));
            }
            tokens.Add(new Token(TokenType.EOF, "", index));
            return tokens;
        }
    }
}
