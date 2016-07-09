namespace Nancy.Helpers
{
    using System;

    internal static class ExceptionExtensions
    {
        internal static Exception FlattenInnerExceptions(this Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                var flattenedAggregateException = aggregateException.Flatten();

                //If we have more than one exception in the AggregateException
                //we have to send all exceptions back in order not to swallow any exceptions.
                if (flattenedAggregateException.InnerExceptions.Count > 1)
                {
                    return flattenedAggregateException;
                }

                return flattenedAggregateException.InnerException;
            }

            return exception;
        }
    }
}