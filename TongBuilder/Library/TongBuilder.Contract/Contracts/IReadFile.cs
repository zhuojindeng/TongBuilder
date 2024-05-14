namespace TongBuilder.Contract.Contracts
{
    public interface IReadFile
    {
        Task<string> ReadContentAsync(string file);

        Task<T> ReadContentAsync<T>(string file);
    }
}
