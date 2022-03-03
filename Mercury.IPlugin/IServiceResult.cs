namespace Mercury.Plugin
{
    public interface IServiceResult
    {
        public string Service { get; set; }
        ResultStatus Status { get; set; }
        string ResultMessage { get; set; }
        dynamic? Result { get; set; }
        bool FromCache { get; set; }
    }
}
