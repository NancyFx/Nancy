namespace Nancy.Helpers
{
    using System.Threading.Tasks;

    public static class TaskHelpers
    {
         public static Task<T> GetCompletedTask<T>(T result)
         {
             var tcs = new TaskCompletionSource<T>();
             tcs.SetResult(result);
             return tcs.Task;
         }
    }
}