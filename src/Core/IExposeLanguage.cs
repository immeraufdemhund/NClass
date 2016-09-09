namespace NClass.Core
{
    public interface IExposeLanguage
    {
        string LanguageName { get; }

        Language Instance { get; }
    }
}
