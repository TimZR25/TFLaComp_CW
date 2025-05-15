namespace TFLaComp_CW.Parsers
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos = 0;
        private readonly List<string> _log = new List<string>();

        private string _input;

        // Follow-множества для синхронизации
        private static readonly HashSet<TokenType> FollowR = new() { TokenType.EOF };
        private static readonly HashSet<TokenType> FollowOB = new() { TokenType.VARIABLE };
        private static readonly HashSet<TokenType> FollowFP = new() { TokenType.COMMA };
        private static readonly HashSet<TokenType> FollowC = new() { TokenType.STAR };
        private static readonly HashSet<TokenType> FollowSP = new() { TokenType.RPAREN };
        private static readonly HashSet<TokenType> FollowCB = new() { TokenType.VARIABLE };
        private static readonly HashSet<TokenType> FollowVS = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowVE = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowE = new() { TokenType.EOF };

        public Parser(List<Token> tokens, string input)
        {
            _tokens = tokens;
            _input = input;
        }

        public List<string> Parse()
        {
            while (_pos < _tokens.Count - 1)
            {
                R();
            }

            if (Current.Type != TokenType.EOF)
                ErrorRecovery(TokenType.EOF, FollowR);

            return _log;
        }

        private Token Current => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EOF, "", _pos, 0);

        private void Advance() => _pos++;

        private void ErrorRecovery(TokenType expected, HashSet<TokenType> syncSet)
        {
            var actual = Current.Type;
            // Попытаться пропустить «ложный» токен
            if (!syncSet.Contains(actual))
            {
                if (actual == TokenType.UNKNOWN)
                {
                    bool isLast = true;
                    string s = Current.Lexeme;
                    string errors = "";
                    int index = 0;

                    for (int i = 0; i < s.Length; i++)
                    {
                        if (char.IsLetterOrDigit(s[i]) == false)
                        {
                            if (isLast == false)
                            {
                                errors += s[i];
                                continue;
                            }

                            isLast = false;

                            int line = Current.Line;
                            int pos = _pos;
                            int l = 0;
                            _pos--;
                            while (_pos > 0 && line == Current.Line)
                            {
                                l += Current.Lexeme.Length;

                                _pos--;
                            }
                            _pos = pos;

                            index = l + i;
                            errors += s[i];
                        }
                        else
                        {
                            if (errors.Length > 0)
                            {
                                _log.Add($"[Delete] line={Current.Line} pos={index} «{errors}» from «{Current.Lexeme}», expected «{expected}»");
                            }

                            errors = "";
                            index = 0;

                            isLast = true;
                        }
                    }
                    if (errors.Length > 0)
                    {
                        _log.Add($"[Delete] line={Current.Line} pos={index} «{errors}» from «{Current.Lexeme}», expected «{expected}»");
                    }

                    Advance();

                    return;
                }
                _log.Add($"[Delete] line={Current.Line} pos={Current.Position} «{actual}», expected «{expected}»");
                Advance();
            }
            else
            {
                // Вставка
                _log.Add($"[Insert] line={Current.Line} pos={Current.Position} inserted «{expected}»");
            }
        }

        private void Match(TokenType expected, HashSet<TokenType> syncSet)
        {
            if (Current.Type == expected)
            {
                Advance();
            }
            else if (Current.Type == TokenType.EOF || syncSet.Contains(Current.Type))
            {
                // вставка
                ErrorRecovery(expected, syncSet);
            }
            else
            {
                // удаление и попытка снова
                ErrorRecovery(expected, syncSet);
                if (Current.Type == expected)
                    Advance();
            }
        }

        // <R> → READ <OB>
        private void R()
        {
            Match(TokenType.READ, FollowR);
            OB();
        }

        // <OB> → '(' <FP>
        private void OB()
        {
            Match(TokenType.LPAREN, FollowOB);
            FP();
        }

        // <FP> → '*' <C>
        private void FP()
        {
            Match(TokenType.STAR, FollowFP);
            C();
        }

        // <C> → ',' <SP>
        private void C()
        {
            Match(TokenType.COMMA, FollowC);
            SP();
        }

        // <SP> → '*' <CB>
        private void SP()
        {
            Match(TokenType.STAR, FollowSP);
            CB();
        }

        // <CB> → ')' <VS>
        private void CB()
        {
            Match(TokenType.RPAREN, FollowCB);
            VS();
        }

        // <VS> → variable <VE>
        private void VS()
        {
            Match(TokenType.VARIABLE, FollowVS);
            VE();
        }

        // <VE> → ',' <VS> | ';' | variable <VE>
        private void VE()
        {
            if (Current.Type == TokenType.COMMA)
            {
                Match(TokenType.COMMA, FollowVE);
                VS();
            }
            else if (Current.Type == TokenType.SEMICOLON)
            {
                E();
            }
            else if (Current.Type == TokenType.VARIABLE)
            {
                Match(TokenType.VARIABLE, FollowVE);
                VS();
            }
            else
            {
                // замена на ';'
                ErrorRecovery(TokenType.SEMICOLON, FollowVE);
            }
        }

        // <E> → ';'
        private void E()
        {
            Match(TokenType.SEMICOLON, FollowE);
        }
    }
}

