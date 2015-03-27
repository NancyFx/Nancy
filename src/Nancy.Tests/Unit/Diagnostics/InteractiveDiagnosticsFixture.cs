namespace Nancy.Tests.Unit.Diagnostics
{
    using Nancy.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
                return string.Concat("In parent public.");
            }
        }

        private class FakeChildDiagnosticsProvider : FakeParentDiagnosticsProvider
        {
            [Description("Child public method.")]
            [Template("<h1>{{model.Result}}</h1>")]
            public string ChildPublicMethod()
            {
                return string.Concat("In child public.");
            }
        }

        [Fact]
        public void Should_return_methods_from_entire_hierarchy()
        {
            //Given
            var child = new FakeChildDiagnosticsProvider();
            var parent = new FakeParentDiagnosticsProvider();
            IEnumerable<IDiagnosticsProvider> ie = new IDiagnosticsProvider[] { child, parent };
            var id = new InteractiveDiagnostics(ie);
            var ad = id.AvailableDiagnostics;

            //When
            int methodsInParent = ad.ElementAt(1).Methods.Count();
            int methodsInChild = ad.First().Methods.Count();

            //Then
            Assert.True(methodsInChild == methodsInParent + 1);
        }
    }
}
