namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class CSharpModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly Type modelType;

        public CSharpModelCodeGenerator(Type modelType)
            : base(modelType.FullName)
        {
            this.modelType = modelType;
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            context.GeneratedClass.UserData.Add("ModelType", this.modelType);

            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}`1[{1}]",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}