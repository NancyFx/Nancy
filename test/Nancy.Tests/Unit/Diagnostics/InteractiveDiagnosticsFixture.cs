namespace Nancy.Tests.Unit.Diagnostics
{
    using Nancy.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class InteractiveDiagnosticsFixture
    {
        private class FakeParentDiagnosticsProvider : IDiagnosticsProvider
        {
            public string Name
            {
                get { return "Fake testing provider"; }
            }

            public string Description
            {
                get { return "Fake testing provider"; }
            }

            public object DiagnosticObject
            {
                get { return this; }
            }

            [Description("Parent public method.")]
            [Template("<h1>{{model.Result}}</h1>")]
            public string ParentPublicMethod()
            {
                return "In parent public.";
            }
        }

        private class FakeChildDiagnosticsProvider : FakeParentDiagnosticsProvider
        {
            [Description("Child public method.")]
            [Template("<h1>{{model.Result}}</h1>")]
            public string ChildPublicMethod()
            {
                return "In child public.";
            }
        }

        [Fact]
        public void Should_return_methods_from_entire_hierarchy()
        {
            //Given
            var child = new FakeChildDiagnosticsProvider();
            var parent = new FakeParentDiagnosticsProvider();

            var diagnostics = new InteractiveDiagnostics(new[] {child, parent});
            var availableDiagnostics = diagnostics.AvailableDiagnostics.ToList();

            //When
            var methodsInChild = availableDiagnostics[0].Methods.Count();
            var methodsInParent = availableDiagnostics[1].Methods.Count();

            //Then
            Assert.True(methodsInChild == methodsInParent + 1);
        }

        [Fact]
        public void Should_exclude_object_methods()
        {
            //Given
            var child = new FakeChildDiagnosticsProvider();

            var diagnostics = new InteractiveDiagnostics(new[] { child });
            var availableDiagnostics = diagnostics.AvailableDiagnostics.ToList();

            var objectMethodNames = typeof(object).GetMethods()
                .Select(x => x.Name)
                .ToList();

            //When
            var methodsInChild = availableDiagnostics[0].Methods;

            //Then
            foreach (var childMethod in methodsInChild)
            {
                Assert.DoesNotContain(childMethod.MethodName, objectMethodNames);
            }
        }
    }
}
