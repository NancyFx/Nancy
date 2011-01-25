using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Nancy.Tests.Unit.Bootstrapper
{
    public class DefaultModuleKeyGeneratorFixture
    {
        private Nancy.Bootstrapper.IModuleKeyGenerator _KeyGenerator;

        /// <summary>
        /// Initializes a new instance of the DefaultModuleKeyGeneratorFixture class.
        /// </summary>
        public DefaultModuleKeyGeneratorFixture()
        {
            _KeyGenerator = new Nancy.Bootstrapper.DefaultModuleKeyGenerator();
        }

        [Fact]
        public void Should_Return_TypeFullName()
        {
            var type = this.GetType();

            var result = _KeyGenerator.GetKeyForModuleType(type);

            result.ShouldBeSameAs(type.FullName);
        }
    }
}
