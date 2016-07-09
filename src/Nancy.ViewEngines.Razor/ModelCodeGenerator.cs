namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser.SyntaxTree;

    /// <summary>
    /// Adds base type with model type as generic argument for Razor view class
    /// </summary>
    public class ModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly Type modelType;

        public ModelCodeGenerator(Type modelType, string typeFullname)
            : base(typeFullname)
        {
            this.modelType = modelType;
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return typeof(object).FullName;
        }

        public override void GenerateCode(Span target, CodeGeneratorContext context)
        {
            base.GenerateCode(target, context);

            context.GeneratedClass.UserData.Add("ModelType", this.modelType);
            context.GeneratedClass.BaseTypes.Clear();
            context.GeneratedClass.BaseTypes.Add(new CodeTypeReference(context.Host.DefaultBaseClass, new CodeTypeReference(this.modelType)));
        }
    }
}