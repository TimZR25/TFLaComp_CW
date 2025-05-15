namespace TFLaComp_CW.Functional
{
    public interface IEdit
    {
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        void Delete();
        void SelectAll();
        Action StateChanged { get; set; }
    }
}
