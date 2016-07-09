namespace Nancy.Tests.xUnitExtensions
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

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

    public static class AssertAsync
    {
        public static async Task<T> Throws<T>(Func<Task> testCode) where T : Exception
        {
            try
            {
                await testCode();
                Assert.Throws<T>(() => { }); // Use xUnit's default behavior.
            }
            catch(T exception)
            {
                return exception;
            }
            return null;
        }
    }
}