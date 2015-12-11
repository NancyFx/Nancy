namespace Nancy.Tests.xUnitExtensions
{
    using System.Collections.Generic;

    using Xunit;
    using Xunit.Sdk;

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
