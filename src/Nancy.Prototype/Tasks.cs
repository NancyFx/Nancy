namespace Nancy.Prototype
{
    using System.Threading.Tasks;

    public static class Tasks
    {
#if NET452
        private static readonly Task Completed = Task.FromResult(0);
#endif

        /// <summary>
        /// Gets a successfully completed, cached <see cref="Task"/>.
        /// </summary>
        public static Task CompletedTask
        {
            get
            {
#if NET452
                return Completed;
#else
                return Task.CompletedTask;
#endif
            }
        }
    }

    public static class Tasks<T>
    {
        /// <summary>
        /// Gets a successfully completed, cached <see cref="Task{T}"/> with the value of <c>default(T)</c>.
        /// </summary>
        public static Task CompletedTask { get; } = Task.FromResult(default(T));
    }
}
