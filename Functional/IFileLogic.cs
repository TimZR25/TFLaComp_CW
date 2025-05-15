namespace TFLaComp_CW.Functional
{
    public interface IFileLogic
    {
        void Create(ref string text);
        void Save(string text);
        void SaveAs(string text);
        string Open();
    }
}