namespace Nancy.Tests
{
    using System;

    /// <summary>
    /// Test helper class that traps any exception that is thrown while executing the specified code expression.
    /// </summary>
    public class Catch
    {
        /// <summary>
        /// Executes the expression specified by the <paramref name="context"/> paramater
        /// and traps any <see cref="Exception"/> that is thrown and returns it.
        /// </summary>
        /// <param name="context">The expression to execute.</param>
        /// <returns>If an exception was trapped then an <see cref="Exception"/> instance is returned; otherwise <see langword="null" />.</returns>
        public static Exception Exception(Action context)
        {
            try
            {
                context();
            }
            catch (Exception thrownException)
            {
                return thrownException;
            }

            return null;
        }
    }
}