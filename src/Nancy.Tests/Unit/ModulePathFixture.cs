namespace Nancy.Tests.Unit
{
    using System;
    using FakeItEasy;
    using Xunit;
    using Nancy.Extensions;

    public class ModulePathFixture
    {
        [Fact]
        public void Should_get_undefined_module_path()
        {  
            // Given
            NancyModule module = new BasicModule();
            var context = new NancyContext();

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldBeEmpty();
            
        }
        [Fact]
        public void Should_get_empty_module_path()
        {  
            // Given
            NancyModule module = new BasicModuleWithEmptyPath();
            var context = new NancyContext();

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual(string.Empty);
            
        }
        [Fact]
        public void Should_get_defined_path()
        {
            // Given

            var request = A.Fake<Request>(x =>
            {
                x.Implements(typeof(IDisposable)); ;
                x.WithArgumentsForConstructor(new[] { "GET", "/defined/path/to/resource", "http" });
            });
            NancyModule module = new DefinedPathModule();
            var context = new NancyContext {Request = request};

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual("/defined");
            
        }
        [Fact]
        public void Should_evaluate_regex_path()
        {  
            // Given

            var request = A.Fake<Request>(x =>
            {
                x.Implements(typeof(IDisposable)); ;
                x.WithArgumentsForConstructor(new[] { "GET", "/path/to/resource", "http" });
            });
            NancyModule module = new RegexPathModule();
            module.Context = new NancyContext();
            var context = new NancyContext { Request = request };

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual("/path");
            
        }
        [Fact]
        public void Should_evaluate_complex_regex_path()
        {  
            // Given

            var request = A.Fake<Request>(x =>
            {
                x.Implements(typeof(IDisposable)); ;
                x.WithArgumentsForConstructor(new[] { "GET", "/path/to/resource", "http" });
            });
            NancyModule module = new ComplexRegexPathModule();
            module.Context = new NancyContext();
            var context = new NancyContext { Request = request };

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual("/path/to/resource");
            
        }

        [Fact]
        public void Should_evaluate_parameterized_path()
        {  
            // Given

            var request = A.Fake<Request>(x =>
            {
                x.Implements(typeof(IDisposable)); ;
                x.WithArgumentsForConstructor(new[] { "GET", "/path/to/resource", "http" });
            });
            NancyModule module = new ParameterizedPathModule();
            module.Context = new NancyContext();
            var context = new NancyContext { Request = request };

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual("/path/to/resource");
            
        }

        [Fact]
        public void Should_return_empty_string_if_request_path_is_shorter_than_module_path()
        {  
            // Given

            var request = A.Fake<Request>(x =>
            {
                x.Implements(typeof(IDisposable)); ;
                x.WithArgumentsForConstructor(new[] { "GET", "long/path/to", "http" });
            });
            NancyModule module = new LongRegexPathModule();
            module.Context = new NancyContext();
            var context = new NancyContext { Request = request };

            // When
            var name = module.GetModulePath(context);

            // Then
            name.ShouldEqual(string.Empty);
            
        }

        class BasicModule : NancyModule
        {
        }

        class EmptyPathModule : NancyModule
        {
            public EmptyPathModule()
                : base("/")
            {
            }
        }
        class BasicModuleWithEmptyPath : NancyModule
        {
            public BasicModuleWithEmptyPath()
                : base("")
            {
            }
        }
        class DefinedPathModule : NancyModule
        {
            public DefinedPathModule()
                : base("/defined")
            {
            }
        }
        class RegexPathModule : NancyModule
        {
            public RegexPathModule()
                : base(@"/(?<path>[\w]+)")
            {
            }
        }
        class LongRegexPathModule : NancyModule
        {
            public LongRegexPathModule()
                : base(@"/long/(?<path>[\w]+)/to/(?<resource>[\w]+)")
            {
            }
        }

        class ComplexRegexPathModule : NancyModule
        {
            public ComplexRegexPathModule()
                : base(@"/(?<path>[\w]+)" + "/to" + @"/(?<resource>[\w]+)")
            {
            }
        }
        class ParameterizedPathModule : NancyModule
        {
            public ParameterizedPathModule()
                : base(@"/{path}/to/{resource}/")
            {
            }
        }
    }
}