namespace Nancy.Helpers
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Convenience class with helper methods for <c>Task</c>s.
    /// </summary>
    public static class TaskHelpers
    {
        /// <summary>
        /// The completed task
        /// </summary>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        /// Gets the faulted task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}