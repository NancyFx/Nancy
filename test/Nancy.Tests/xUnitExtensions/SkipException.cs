// Copyright (c) 2015 Andrew Arnott
// Licensed under the Ms-PL

namespace Nancy.Tests.xUnitExtensions
{
    using System;

    /// <summary>
    /// The exception to throw to register a skipped test.
    /// </summary>
    public class SkipException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipException"/> class.
        /// </summary>
        /// <param name="reason">The reason the test is skipped.</param>
        public SkipException(string reason)
            : base(reason)
        {
        }
    }
}
