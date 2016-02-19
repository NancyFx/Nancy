namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Razor.Tokenizer.Symbols;

    /// <summary>
    /// Resolves C# type name to CLR type
    /// </summary>
    internal class CSharpClrTypeResolver : ClrTypeResolver<CSharpSymbolType, CSharpSymbol>
    {
        public CSharpClrTypeResolver(RazorAssemblyProvider razorAssemblyProvider)
            : base(razorAssemblyProvider, CSharpSymbolType.Identifier, CSharpSymbolType.Keyword, CSharpSymbolType.Dot, CSharpSymbolType.WhiteSpace, CSharpSymbolType.LeftBracket, CSharpSymbolType.RightBracket)
        {
        }

        /// <summary>
        /// Dequeues symbols '>' representing end of generic arguments
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected override bool MoveOutOfGenericArguments()
        {
            if (this.Symbols.Peek().Type == CSharpSymbolType.GreaterThan)
            {
                this.Symbols.Dequeue();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Dequeues symbol ',' and whitespace representing separator between generic arguments
        /// </summary>
        protected override void MoveToNextGenericArgument()
        {
            while (this.Symbols.Peek().Type == CSharpSymbolType.WhiteSpace || this.Symbols.Peek().Type == CSharpSymbolType.Comma)
            {
                this.Symbols.Dequeue();
            }
        }

        /// <summary>
        /// equeues symbol '&lt;' representing begin of generic arguments
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected override bool MoveToGenericArguments()
        {
            if (this.Symbols.Peek().Type != CSharpSymbolType.LessThan)
            {
                return false;
            }

            this.Symbols.Dequeue();

            return true;
        }

        /// <summary>
        /// Gets CLR from name (keyword) used by C#
        /// </summary>
        /// <param name="typeName">Type name to resolve</param>
        /// <returns>CLR type</returns>
        protected override Type ResolvePrimitiveType(string typeName)
        {
            var primitives = new Dictionary<string, Type>
            {
                {"string", typeof (string)},
                {"byte", typeof (byte)},
                {"sbyte", typeof (sbyte)},
                {"short", typeof (short)},
                {"ushort", typeof (ushort)},
                {"int", typeof (int)},
                {"uint", typeof (uint)},
                {"long", typeof (long)},
                {"ulong", typeof (ulong)},
                {"float", typeof (float)},
                {"double", typeof (double)},
                {"decimal", typeof (decimal)},
                {"char", typeof (char)},
                {"bool", typeof (bool)},
                {"object", typeof (object)},
            };

            return (primitives.ContainsKey(typeName) ? primitives[typeName] : null);
        }
    }
}