namespace YAB.Plugins.Injectables.Options
{
    public interface IOptions
    {
        void Load(string password);

        void Save(string password);
    }
}