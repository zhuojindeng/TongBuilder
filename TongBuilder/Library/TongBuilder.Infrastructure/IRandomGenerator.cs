namespace TongBuilder.Infrastructure
{
    public interface IRandomGenerator
    {
        int Next(int minValue, int maxValue);

        double NextDouble();
    }
}
