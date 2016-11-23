#if !CORE
namespace Nancy.Tests.Unit
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class AppDomainAssemblyCatalogFixture
    {
        [Fact]
        public void Modules_without_Nancy_references_should_not_keep_loaded_after_inspection()
        {
            // Given
            var compilerAppDomain = AppDomain.CreateDomain("AssemblyGenerator",
                AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

            var assemblyGenerator = (ProxyAssemblyGenerator)compilerAppDomain.CreateInstanceAndUnwrap(
                typeof(ProxyAssemblyGenerator).Assembly.FullName,
                typeof(ProxyAssemblyGenerator).FullName);

            try
            {
                var generatedAssemblyName = assemblyGenerator.GenerateAssemblyAndGetName();

                var assemblyCatalog = new AppDomainAssemblyCatalog();

                // When

                // the following call will load the assemblies into its own inspection AppDomain
                // and release the assemblies that do not reference Nancy afterwards, 
                // keeping the application AppDomain free of such assemblies, that is, the created
                // assembly should not be loaded by AppDomain.GetAssemblies after this call
                assemblyCatalog.GetAssemblies();

                var loadedAssembliesAfterInspection = AppDomain.CurrentDomain.GetAssemblies();

                // Then
                loadedAssembliesAfterInspection
                    .Select(assembly => assembly.GetName().Name)
                    .Contains(generatedAssemblyName.Name)
                    .ShouldBeFalse();
            }
            finally
            {
                AppDomain.Unload(compilerAppDomain);
            }
        }

        private class ProxyAssemblyGenerator : MarshalByRefObject
        {
            public AssemblyName GenerateAssemblyAndGetName()
            {
                var generatedAssembly = CodeDomProvider
                    .CreateProvider("CSharp")
                    .CompileAssemblyFromSource(
                        new CompilerParameters
                        {
                            GenerateInMemory = true,
                            GenerateExecutable = false,
                            IncludeDebugInformation = false,
                            OutputAssembly = "AssemblyShouldNotBeLoadedIntoAppDomain.dll"
                        },
                        "public class DummyClass { }")
                    .CompiledAssembly;

                return generatedAssembly.GetName();
            }
        }
    }
}
#endif
