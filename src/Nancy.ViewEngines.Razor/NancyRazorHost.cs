using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Directives;
using Microsoft.AspNetCore.Razor.Chunks;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;

namespace Nancy.ViewEngines.Razor
{
    public class NancyRazorHost : MvcRazorHost
    {
        private const string BaseType = "Nancy.ViewEngines.Razor.NancyRazorPage";

        public NancyRazorHost(IRazorConfiguration configuration, IChunkTreeCache chunkTreeCache, ITagHelperDescriptorResolver resolver) : base(chunkTreeCache, resolver)
        {
            DefaultBaseClass = $"{BaseType}<{ChunkHelper.TModelToken}>";
            DefaultInheritedChunks.OfType<SetBaseTypeChunk>().First().TypeName = DefaultBaseClass;  // The chunk is actually what injects the base name into the view
            EnableInstrumentation = false;

            // Add additional default namespaces from the execution context
            foreach (string ns in configuration.GetDefaultNamespaces())
            {
                NamespaceImports.Add(ns);
            }
        }
    }
}