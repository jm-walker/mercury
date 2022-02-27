namespace Mercury.IPlugin
{
    public interface IPluginResult
    {
        ResultStatus Status { get; set; }
        string ResultMessage { get; set; }
        string Result { get; set; }
    }
}
