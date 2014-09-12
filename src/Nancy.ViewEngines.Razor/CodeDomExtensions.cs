namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom;
    using System.Text;

    internal static class CodeDomExtensions
    {
        public static Type AsClrType(this CodeTypeReference @this)
        {
            var typeName = @this.AsClrTypeName();

            return Type.GetType(typeName);
        }

        public static string AsClrTypeName(this CodeTypeReference @this)
        {
            if (@this.TypeArguments.Count == 0)
            {
                return @this.BaseType;
            }

            return "";
        }
    }
}
