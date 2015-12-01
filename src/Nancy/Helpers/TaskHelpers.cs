namespace Nancy.Helpers
{
    using System;
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
        private static readonly Lazy<Task> CompletedTask = new Lazy<Task>(() => GetCompletedTask<object>(null));

        public static Task GetCompletedTask()
        {
            return CompletedTask.Value;
        }

        public static Task<T> GetCompletedTask<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}