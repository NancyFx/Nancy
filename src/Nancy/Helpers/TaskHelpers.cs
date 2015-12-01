namespace Nancy.Helpers
{
    using System;
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
        private static readonly Lazy<Task> CompletedTask = new Lazy<Task>(() => Task.FromResult<object>(null));

        public static Task GetCompletedTask()
        {
            return CompletedTask.Value;
        }

        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}