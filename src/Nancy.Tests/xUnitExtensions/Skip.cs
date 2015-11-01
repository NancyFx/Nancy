// Copyright (c) 2015 Andrew Arnott
// Licensed under the Ms-PL

namespace Nancy.Tests.xUnitExtensions
{
    /// <summary>
    /// Static methods for dynamically skipping tests identified with
    /// the <see cref="SkippableFactAttribute"/>.
    /// </summary>
    public static class Skip
    {
        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to <c>true</c> for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void If(bool condition, string reason = null)
        {
            if (condition)
            {
                throw new SkipException(reason);
            }
        }

        /// <summary>
        /// Throws an exception that results in a "Skipped" result for the test.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to <c>false</c> for the test to be skipped.</param>
        /// <param name="reason">The explanation for why the test is skipped.</param>
        public static void IfNot(bool condition, string reason = null)
        {
            Skip.If(!condition, reason);
        }
    }
}