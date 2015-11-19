namespace Nancy.Tests.xUnitExtensions
{
    using System;
    using System.Threading.Tasks;

    public static class RecordAsync
    {
        public static async Task<Exception> Exception(Func<Task> method)
        {
            try
            {
                await method();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}