namespace Nancy.Testing.Tests
{
    using System;

    using FakeItEasy;

    using Xunit;

    public class ConfigurableBootstrapperDependenciesTests
    {
        private const string _dataFromFake = "Faked version ITestDependency";
        private const string _dataFromFake2 = "Faked version ITestDependency2";

        private static ITestDependency GetFakeDependency()
        {
            var fakeRep = A.Fake<ITestDependency>();
            A.CallTo(() => fakeRep.GetData()).Returns(_dataFromFake);
            return fakeRep;
        }

        private static ITestDependency2 GetFakeDependency2()
        {
            var fakeRep = A.Fake<ITestDependency2>();
            A.CallTo(() => fakeRep.GetData()).Returns(_dataFromFake2);
            return fakeRep;
        }

        [Fact]
        public void should_be_able_to_configure_dependency_typeparam()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<ModuleWithOneDependency>();
                with.Dependency<ImplementedTestDependency>();
            });

            // When
            var response = browser.Get("/1dependency");

            // Assert
            Assert.Contains("Implemented ITestDependency", response.Body.AsString());
        }

        [Fact]
        public void should_be_able_to_configure_dependency_typeparam_and_instance()
        {
            // Given
            var browser = new Browser(with =>
                    {
                        with.Module<ModuleWithOneDependency>();
                        with.Dependency<ITestDependency>(GetFakeDependency());
                    });

            // When
            var response = browser.Get("/1dependency");

            // Assert
            Assert.Contains(_dataFromFake, response.Body.AsString());
        }

        [Fact]
        public void should_be_able_to_configure_dependency_instance()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<ModuleWithOneDependency>();
                with.Dependency(GetFakeDependency());
            });

            // When
            var response = browser.Get("/1dependency");

            // Assert
            Assert.Contains(_dataFromFake, response.Body.AsString());
        }

        [Fact]
        public void should_be_able_to_configure_dependency_type()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<ModuleWithOneDependency>();
                with.Dependency<ITestDependency>(typeof(ImplementedTestDependency));
            });

            // When
            var response = browser.Get("/1dependency");

            // Assert
            Assert.Contains("Implemented ITestDependency", response.Body.AsString());
        }

        interface IIinterface { }
        class DisposableDependency : IIinterface, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                this.Disposed = true;
            }
        }

        [Fact]
        public void should_be_able_to_configure_disposbale_dependency_implementing_interface_by_interface()
        {
            // Given
            var disposableDependency = new DisposableDependency();

            // When
            new Browser(with =>
              with.Dependency<IIinterface>(disposableDependency)
            );

            // Then
            Assert.False(disposableDependency.Disposed);
        }


        [Fact]
        public void should_be_able_to_configure_dependencies_by_instances()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<ModuleWithTwoDependencies>();
                with.Dependencies<object>(GetFakeDependency(), GetFakeDependency2());
            });

            // When
            var response = browser.Get("/2dependencies");

            // Assert
            var bodyAsString = response.Body.AsString();
            Assert.Contains(_dataFromFake, bodyAsString);
            Assert.Contains(_dataFromFake2, bodyAsString);
        }

        [Fact]
        public void should_be_able_to_configure_dependencies_by_a_map_with_interface_and_instance()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<ModuleWithTwoDependencies>();
                with.MappedDependencies(new[] {
                                       new Tuple<Type, object>(typeof(ITestDependency), GetFakeDependency()),
                                       new Tuple<Type, object>(typeof(ITestDependency2), GetFakeDependency2())
                                   });
            });

            // When
            var response = browser.Get("/2dependencies");

            // Assert
            var bodyAsString = response.Body.AsString();
            Assert.Contains(_dataFromFake, bodyAsString);
            Assert.Contains(_dataFromFake2, bodyAsString);
        }


        public class ModuleWithOneDependency : NancyModule
        {
            public ModuleWithOneDependency(ITestDependency dependency)
                : base("/1dependency")
            {
                Get["/"] = _ =>
                {
                    return string.Format("Data of dependency: {0}", dependency.GetData());
                };
            }
        }

        public class ModuleWithTwoDependencies : NancyModule
        {
            public ModuleWithTwoDependencies(ITestDependency dependency, ITestDependency2 dependency2)
                : base("/2dependencies")
            {
                Get["/"] = _ =>
                {
                    return string.Format("Data of dependencies: {0}, {1}", dependency.GetData(), dependency2.GetData());
                };
            }
        }

        public interface ITestDependency
        {
            string GetData();
        }

        public class ImplementedTestDependency : ITestDependency
        {
            public string GetData()
            {
                return "Implemented ITestDependency";
            }
        }

        public interface ITestDependency2
        {
            string GetData();
        }

        public class ImplementedTestDependency2 : ITestDependency2
        {
            public string GetData()
            {
                return "Implemented ITestDependency2";
            }
        }
    }
}