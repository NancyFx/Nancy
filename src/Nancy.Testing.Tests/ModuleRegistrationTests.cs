using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nancy.Testing.Tests
{
    public class ModuleRegistrationTests 
    {
        [Fact]
        public void test_module_resolves_from_previous_container_registration()
        {
            Browser browser = new Browser(new ModuleRegistrationBootStrap());
            string responseText = browser.Get("/test").Body.AsString();
            Assert.Equal("return value", responseText);
        }
    }

    public class ModuleRegistrationBootStrap : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<TestModule>(new TestModule("return value"));
        }
    }

    public class TestModule : NancyModule
    {
        public TestModule(string returnValue)
        {
            Get["/test"] = _ => returnValue;
        }
    }
}
