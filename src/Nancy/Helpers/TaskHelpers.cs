namespace Nancy.Helpers
{
    using System;
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
        private static readonly Lazy<Task> LazyCompletedTask = new Lazy<Task>(() => Task.FromResult<object>(null));

        public static Task CompletedTask
        {
            get { return LazyCompletedTask.Value; }
        }

        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}