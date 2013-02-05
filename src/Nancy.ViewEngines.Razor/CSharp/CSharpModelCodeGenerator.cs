namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class CSharpModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        public CSharpModelCodeGenerator(string baseType)
            : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}