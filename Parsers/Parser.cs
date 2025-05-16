namespace TFLaComp_CW.Parsers
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos = 0;
        public List<string> Logs { get; } = new List<string>();

        // Follow-множества для синхронизации
        private static readonly HashSet<TokenType> FollowR = new() { TokenType.OBRACKET};
        private static readonly HashSet<TokenType> FollowOB = new() { TokenType.STAR};
        private static readonly HashSet<TokenType> FollowFP = new() { TokenType.COMMA };
        private static readonly HashSet<TokenType> FollowC = new() { TokenType.STAR };
        private static readonly HashSet<TokenType> FollowSP = new() { TokenType.CBRACKET };
        private static readonly HashSet<TokenType> FollowCB = new() { TokenType.VARIABLE };
        private static readonly HashSet<TokenType> FollowVS = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowVE = new() { TokenType.SEMICOLON };
        private static readonly HashSet<TokenType> FollowE = new() { TokenType.EOF };

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public void Parse()
        {
            while (_pos < _tokens.Count - 1)
            {
                R();
            }

            if (Current.Type != TokenType.EOF)
                ErrorRecovery(TokenType.EOF, FollowR);
        }

        private Token Current => _pos < _tokens.Count ? _tokens[_pos] : new Token(TokenType.EOF, "", _pos, 0);

        private void ErrorRecovery(TokenType expected, HashSet<TokenType> syncSet)
        {
            var actual = Current.Type;

            if (actual == TokenType.EOF)
            {
                Logs.Add($"[Грамматическая ошибка] строка: {Current.Line} позиция: {Current.Position} вставлен «{Token.GetTypeName(expected)}»");

                return;
            }

            // Попытаться пропустить «ложный» токен
            if (!syncSet.Contains(actual))
            {
                int pos = _pos;
                List<string> logs = new List<string>();

                int t = _pos;
                while (Current.Type != expected && t < _tokens.Count)
                {
                    logs.Add($"[Грамматическая ошибка] строка: {Current.Line} позиция: {Current.Position} удалена «{Current.Lexeme}»");

                    _pos++;

                    t++;
                }

                if (Current.Type == expected)
                {
                    Logs.AddRange(logs);
                }
                else
                {
                    _pos = pos;

                    Logs.Add($"[Грамматическая ошибка] строка: {Current.Line} позиция: {Current.Position} «{Current.Lexeme}» заменена на «{Token.GetTypeName(expected)}»");
                    _pos++;
                }

            }
            else
            {
                _pos++;
                if (syncSet.Contains(Current.Type))
                {
                    _pos--;
                    Logs.Add($"[Грамматическая ошибка] строка: {Current.Line} позиция: {Current.Position} «{Current.Lexeme}» заменена на «{Token.GetTypeName(expected)}»");

                    _pos++;
                    return;
                }
                else
                {
                    _pos--;
                    // Вставка
                    Logs.Add($"[Грамматическая ошибка] строка: {Current.Line} позиция: {Current.Position} вставлен «{Token.GetTypeName(expected)}»");
                }
            }
        }

        private void Match(TokenType expected, HashSet<TokenType> syncSet)
        {
            if (Current.Type == expected)
            {
                _pos++;
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
                    _pos++;
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
            Match(TokenType.OBRACKET, FollowOB);
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
            Match(TokenType.CBRACKET, FollowCB);
            VS();
        }

        // <VS> → variable <VE>
        private void VS()
        {
            Match(TokenType.VARIABLE, FollowVS);
            VE();
        }

        // <VE> → ',' <VS> | ';'
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
                ErrorRecovery(TokenType.SEMICOLON, FollowVE);
                E();
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

