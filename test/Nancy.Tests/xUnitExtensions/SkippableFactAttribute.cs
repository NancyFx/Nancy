// Copyright (c) 2015 Andrew Arnott
// Licensed under the Ms-PL

namespace Nancy.Tests.xUnitExtensions
{
    using System;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should
    /// be run by the test runner.
    /// The test may produce a "skipped test" result by calling
    /// <see cref="Skip.If(bool, string)"/> or otherwise throwing a <see cref="SkipException"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Sdk.SkippableFactDiscoverer", "Xunit.SkippableFact.{Platform}")]
    public class SkippableFactAttribute : FactAttribute
    {
    }
}
