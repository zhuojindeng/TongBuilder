namespace TongBuilder.Contract.Plugins
{
    public interface ISMSSender
    {
        Task SendSMSAsync(string signName, string phone, string templateCode, Dictionary<string, string> templateParams);
    }
}
