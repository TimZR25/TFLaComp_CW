namespace TFLaComp_CW.Parsers
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos = 0;
        private readonly List<string> _log = new List<string>();

        // Follow-множества для синхронизации
        private static readonly HashSet<TokenType> FollowR = new() { TokenType.EOF };
        private static readonly HashSet<TokenType> FollowOB = new() { TokenType.LETTER };
        private static readonly HashSet<TokenType> FollowFP = new() { TokenType.COMMA };
        private static readonly HashSet<TokenType> FollowC = new() { TokenType.STAR };
        private static readonly HashSet<TokenType> FollowSP = new() { TokenType.RPAREN };
        private static readonly HashSet<TokenType> FollowCB = new() { TokenType.LETTER };
        private static readonly HashSet<TokenType> FollowVS = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowVE = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowE = new() { TokenType.EOF };

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<string> Parse()
        {
            R();
            if (Current.Type != TokenType.EOF)
                ErrorRecovery(TokenType.EOF, FollowR);
            Console.WriteLine("Разбор завершён. Журнал корректировок:");

            return _log;
        }

        private Token Current => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EOF, "", _pos);

        private void Advance() => _pos++;

        private void ErrorRecovery(TokenType expected, HashSet<TokenType> syncSet)
        {
            var actual = Current.Type;
            // Попытаться пропустить «ложный» токен
            if (!syncSet.Contains(actual))
            {
                _log.Add($"[Delete] pos={Current.Position} «{actual}», expected «{expected}»");
                Advance();
            }
            else
            {
                // Вставка
                _log.Add($"[Insert] pos={Current.Position} inserted «{expected}»");
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

        // <VS> → LETTER <VE>
        private void VS()
        {
            Match(TokenType.LETTER, FollowVS);
            VE();
        }

        // <VE> → ',' <VS> | ';' | LETTER <VE> | DIGIT <VE>
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
            else if (Current.Type == TokenType.LETTER)
            {
                Match(TokenType.LETTER, FollowVE);
                VE();
            }
            else if (Current.Type == TokenType.DIGIT)
            {
                Match(TokenType.DIGIT, FollowVE);
                VE();
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

