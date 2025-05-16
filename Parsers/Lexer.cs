using Microsoft.VisualBasic.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFLaComp_CW.Parsers
{
    public class Lexer
    {
        public List<string> Logs { get; } = new List<string>();

        public List<Token> GetTokens(string input)
        {
            Logs.Clear();

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
                    .Replace("*", " * ")
                    .Split(new[] { ' ', '\t', '\v' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i++, index++)
                {
                    string s = parts[i];

                    TokenType type = GetType(s);

                    string value = s;

                    List<(int, int)> errorsIndexes = new List<(int, int)>();

                    if (type == TokenType.UNKNOWN)
                    {
                        value = "";

                        bool isLast = true;

                        int startIndex = 0;
                        int length = 0;

                        for (int j = 0; j < s.Length; j++)
                        {
                            if (char.IsLetterOrDigit(s[j]) == false)
                            {
                                if (isLast == false)
                                {
                                    length++;
                                    continue;
                                }

                                isLast = false;

                                startIndex = j;
                                length = 1;
                            }
                            else
                            {
                                value += s[j];

                                if (length > 0)
                                {
                                    errorsIndexes.Add((startIndex, length));

                                    isLast = true;
                                    length = 0;
                                }
                            }
                        }
                        if (length > 0)
                        {
                            errorsIndexes.Add((startIndex, length));
                        }

                        string error;

                        for (int j = 0; j < errorsIndexes.Count; j++)
                        {
                            startIndex = errorsIndexes[j].Item1;
                            length = errorsIndexes[j].Item2;

                            error = s.Substring(startIndex, length);

                            Logs.Add($"[Лексическая ошибка] Строка: {l + 1} позиция: {index} удаление неспециализированных символов «{error}» из «{s}»");
                        }

                        type = GetType(value);
                    }

                    if (string.IsNullOrEmpty(value))
                    {
                        value = s;
                    }

                    if (type == TokenType.UNKNOWN)
                    {
                        if (char.IsLetter(value[0]) == true)
                        {
                            type = TokenType.VARIABLE;
                        }
                    }

                    tokens.Add(new Token(type, value, index, l + 1));
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", index, lines.Length));

            return tokens;
        }

        private TokenType GetType(string value)
        {
            string ls = value.ToLower();
            TokenType type = ls switch
            {
                "read" => TokenType.READ,
                "(" => TokenType.OBRACKET,
                ")" => TokenType.CBRACKET,
                "*" => TokenType.STAR,
                "," => TokenType.COMMA,
                ";" => TokenType.SEMICOLON,
                _ => TokenType.UNKNOWN
            };

            return type;
        }
    }
}
