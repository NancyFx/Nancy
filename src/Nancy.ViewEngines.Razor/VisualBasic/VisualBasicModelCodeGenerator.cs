namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class VisualBasicModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly Type modelType;

        public VisualBasicModelCodeGenerator(Type modelType)
            : base(modelType.FullName)
        {
            this.modelType = modelType;
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            context.GeneratedClass.UserData["ModelType"] = modelType;

            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}`1[{1}]",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}