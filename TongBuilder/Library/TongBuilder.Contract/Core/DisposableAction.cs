namespace TongBuilder.Contract.Core
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            Check.NotNull(action, nameof(action));

            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
