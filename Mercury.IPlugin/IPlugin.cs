namespace Mercury.Plugin
{
    public interface IPlugin
    {
        public string Name { get; }
        Task<IServiceResult> QueryURL(string url);
    }
}