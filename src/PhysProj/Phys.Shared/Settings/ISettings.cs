namespace Phys.Shared.Settings
{
    public interface ISettings
    {
        Type GetType(string code);

        void Register<T>(string code, T defaultValue);

        void Set(string code, object value);

        object Get(string code);

        List<string> List();
    }
}
