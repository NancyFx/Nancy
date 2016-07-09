namespace Nancy.Tests.Helpers
{
    using System;
    using Nancy.Helpers;
    using Xunit;

    public class ExceptionExtensionsFixture
    {
        [Fact]
        public void Should_return_exception_if_not_aggregate_exception()
        {
            var exception = new Exception("Not an aggregate exception.", new Exception("Inner exception."));
            var result = exception.FlattenInnerExceptions();

            Assert.Equal(exception, result);
        }

        [Fact]
        public void Should_flatten_aggregate_exceptions()
        {
            var exception1 = new Exception("Exception 1", new Exception("Inner exception of exception 1"));
            var exception2 = new Exception("Exception 2", new Exception("Inner exception of exception 2"));
            var exception3 = new Exception("Exception 3", new Exception("Inner exception of exception 3"));
            
            // Aggregate exceptions nested three levels deep.
            var aggregate3 = new AggregateException(exception3);
            var aggregate2 = new AggregateException(aggregate3, exception2);
            var aggregate1 = new AggregateException(aggregate2, exception1);

            var result = aggregate1.FlattenInnerExceptions();

            Assert.IsType<AggregateException>(result);

            // Only the inner exceptions of any aggregates should be returned. The inner exception
            // of a non-aggregate should not be flattened.
            var innerExceptions = ((AggregateException)result).InnerExceptions;
            var expectedExceptions = new[] { exception1, exception2, exception3 };

            Assert.Equal(3, innerExceptions.Count);

            foreach (var exception in expectedExceptions)
                Assert.True(innerExceptions.Contains(exception));
        }
    }
}
