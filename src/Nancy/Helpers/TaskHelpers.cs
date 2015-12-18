namespace Nancy.Helpers
{
    using System;
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}