using TFLaComp_1.Functional;
using TFLaComp_1.ParserHelp;
using TFLaComp_CW.Functional;
using TFLaComp_CW.Parsers;

namespace TFLaComp_1
{
    public enum ParserType
    {
        Конечный_автомат, Регулярное_выражение
    }

    public partial class ParserForm : Form
    {
        private IEdit _edit;

        private IFileLogic _logic;

        private IParserHelpProvider _helpProvider;

        private bool isTextChanged = false;

        public ParserForm()
        {
            InitializeComponent();
            
            InitEdit();
            _logic = new FileLogic();

            SetHelp();

            richTextBoxInput.TextChanged += richTextBoxInput_TextChanged;
        }

        private void SetHelp()
        {
            HelpButton = true;

            _helpProvider = new ParserHelpProvider();
            _helpProvider.SetHelp(richTextBoxInput, HelpHtmDict.TopicDict["Правка"], HelpNavigator.Topic);
            _helpProvider.SetHelp(file, "Создать", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(open, "Открыть", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(save, "Сохранить", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(start, "Пуск", HelpNavigator.Topic);

            _helpProvider.SetHelp(undo, "Отменить", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(redo, "Повторить", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(copy, "Копировать", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(cut, "Вырезать", HelpNavigator.KeywordIndex);
            _helpProvider.SetHelp(paste, "Вставить", HelpNavigator.KeywordIndex);

            _helpProvider.SetHelp(this, "О программе", HelpNavigator.KeywordIndex);

            helpProvider1 = _helpProvider.HelpProvider;
        }

        private void ProcessInput()
        {
            ClearOutput();

            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.GetTokens(richTextBoxInput.Text);
            var parser = new Parser(tokens, richTextBoxInput.Text);
            try
            {
                var log = parser.Parse();

                foreach (var token in log)
                {
                    richTextBoxOutput.Text += token + "\n";
                }
            }
            catch (Exception ex)
            {
                richTextBoxOutput.Text = "Fatal: " + ex.Message;
            }

            foreach (var token in tokens)
            {
                richTextBoxTokens.Text += $" {token.Line} | {token.Position} | {token.Lexeme} | {token.Type} \n";
            }

            richTextBoxInput.ClearUndo();
            OnStateChanged();
        }

        private void makeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConfirmSaveChanges())
            {
                string text = richTextBoxInput.Text;
                _logic.Create(ref text);
                richTextBoxInput.Text = text;
                ClearOutput();
            }
        }

        private void file_Click(object sender, EventArgs e)
        {
            if (ConfirmSaveChanges())
            {
                string text = richTextBoxInput.Text;
                _logic.Create(ref text);
                richTextBoxInput.Text = text;
                ClearOutput();
            }
        }

        private void open_Click(object sender, EventArgs e)
        {
            if (ConfirmSaveChanges())
                LoadFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConfirmSaveChanges())
                LoadFile();
        }

        private bool ConfirmSaveChanges()
        {
            if (!isTextChanged || richTextBoxInput.Text == "") return true; // Если изменений нет, просто продолжаем.

            DialogResult result = MessageBox.Show(
                "Сохранить изменения перед открытием нового файла?",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _logic.Save(richTextBoxInput.Text);
                isTextChanged = false;
                return true;
            }
            else if (result == DialogResult.No)
            {
                return true;
            }

            return false; // Отмена операции
        }

        private void LoadFile()
        {
            try
            {
                string? text = _logic.Open() ?? throw new FileLoadException("Ошибка открытия файла!");
                richTextBoxInput.Text = text;
                isTextChanged = false; // Сбрасываем флаг изменений
                InitEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            _logic.Save(richTextBoxInput.Text);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logic.Save(richTextBoxInput.Text);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logic.SaveAs(richTextBoxInput.Text);
        }

        private void ClearOutput()
        {
            richTextBoxOutput.Clear();
            richTextBoxTokens.Clear();
        }

        private void start_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void undo_Click(object sender, EventArgs e)
        {
            _edit.Undo();
        }

        private void redo_Click(object sender, EventArgs e)
        {
            _edit.Redo();
        }

        private void cut_Click(object sender, EventArgs e)
        {
            _edit.Cut();
        }

        private void paste_Click(object sender, EventArgs e)
        {
            _edit.Paste();
        }

        private void copy_Click(object sender, EventArgs e)
        {
            _edit.Copy();
        }

        private void callHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace);
        }

        private void aboutCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.KeywordIndex, "О программе");
        }

        private void expToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Постановка задачи"]);
        }

        private void grammarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Грамматика"]);

        }

        private void classificationgrammarClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Классификация"]);

        }

        private void analysismethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Метод анализа"]);

        }

        private void diagnosticsNeutralizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Диагностика"]);

        }

        private void explToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Тестовый пример"]);

        }

        private void bibliographyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Список литературы"]);

        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // потом должен открываться файл курсовой
            Help.ShowHelp(this, helpProvider1.HelpNamespace,
                HelpNavigator.Topic, HelpHtmDict.TopicDict["Исходный код"]);

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.Delete();
        }

        private void pasteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _edit.SelectAll();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы хотите сохранить перед выходом?",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _logic.Save(richTextBoxInput.Text);
            }
            else if (result == DialogResult.No)
            {
                Application.Exit();
            }
        }

        private void richTextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                _edit.Undo();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                _edit.Redo();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                _edit.SelectAll();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                _edit.Cut();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                _edit.Copy();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                _edit.Paste();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                _logic.Save(richTextBoxInput.Text);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                _logic.Open();
                e.SuppressKeyPress = true;
            }
        }

        private void richTextBoxInput_TextChanged(object sender, EventArgs e)
        {
            OnStateChanged();

            isTextChanged = true;
        }

        private void InitEdit()
        {
            _edit = new Edit(richTextBoxInput);
            _edit.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            undo.Enabled = richTextBoxInput.CanUndo;
            redo.Enabled = richTextBoxInput.CanRedo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
