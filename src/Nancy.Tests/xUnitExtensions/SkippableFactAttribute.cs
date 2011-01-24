using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace Nancy.Tests.xUnitExtensions
{
    public class SkippableFactAttribute : FactAttribute
    {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            yield return new SkippableTestCommand(method);
        }

        class SkippableTestCommand : FactCommand
        {
            public SkippableTestCommand(IMethodInfo method) : base(method) { }

            public override MethodResult Execute(object testClass)
            {
                try
                {
                    return base.Execute(testClass);
                }
                catch (SkipException e)
                {
                    return new SkipResult(testMethod, DisplayName, e.Reason);
                }
            }
        }
    }
}
