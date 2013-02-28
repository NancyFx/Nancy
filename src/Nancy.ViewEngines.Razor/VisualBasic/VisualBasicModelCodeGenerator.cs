namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class VisualBasicModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        public VisualBasicModelCodeGenerator(string baseType)
            : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}(Of {1})",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}