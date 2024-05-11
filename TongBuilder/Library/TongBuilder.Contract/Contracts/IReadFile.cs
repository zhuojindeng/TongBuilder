namespace TongBuilder.Contract.Contracts
{
    public interface IReadFile
    {
        Task<string> ReadContent(string file);
    }
}
