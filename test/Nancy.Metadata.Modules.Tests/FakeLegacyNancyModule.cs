namespace Nancy.Metadata.Modules.Tests
{
    using System;

    public class FakeLegacyNancyModule : LegacyNancyModule
    {
        public FakeLegacyNancyModule()
        {
        }

        public FakeLegacyNancyModule(Action<FakeNancyModuleConfigurator> closure)
        {
            var configurator = 
                new FakeNancyModuleConfigurator(this);

            closure.Invoke(configurator);
        }

        public class FakeNancyModuleConfigurator
        {
            private readonly LegacyNancyModule module;

            public FakeNancyModuleConfigurator(LegacyNancyModule module)
            {
                this.module = module;
            }

            public FakeNancyModuleConfigurator AddDeleteRoute(string path)
            {
                this.module.Delete[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }

            public FakeNancyModuleConfigurator AddGetRoute(string path)
            {
                return this.AddGetRoute(path, x => HttpStatusCode.OK);
            }

            public FakeNancyModuleConfigurator AddGetRoute(string path, Func<object, Response> action)
            {
                this.module.Get[path] = action;
                return this;
            }

            public FakeNancyModuleConfigurator AddPostRoute(string path)
            {
                this.module.Post[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }

            public FakeNancyModuleConfigurator AddPutRoute(string path)
            {
                this.module.Put[path] = parameters => {
                    return HttpStatusCode.OK;
                };

                return this;
            }
        }
    }
}