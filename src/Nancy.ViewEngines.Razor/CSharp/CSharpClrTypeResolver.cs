namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Razor.Tokenizer.Symbols;

    internal class CSharpClrTypeResolver : ClrTypeResolver<CSharpSymbolType, CSharpSymbol>
    {
        public CSharpClrTypeResolver()
            : base(CSharpSymbolType.Identifier, CSharpSymbolType.Keyword, CSharpSymbolType.Dot, CSharpSymbolType.WhiteSpace)
        {
        }

        protected override TypeNameParserStep ResolveType()
        {
            var identifier = this.PopFullIdentifier();

            var step = new TypeNameParserStep(identifier);

            if (!this.symbols.Any()) return step;

            if (this.symbols.Peek().Type == CSharpSymbolType.LessThan)
            {
                this.symbols.Dequeue();

                while (this.symbols.Peek().Type != CSharpSymbolType.GreaterThan)
                {
                    step.GenericArguments.Add(this.ResolveType());

                    while (this.symbols.Peek().Type.Equals(this.WhiteSpace) || this.symbols.Peek().Type == CSharpSymbolType.Comma)
                    {
                        this.symbols.Dequeue();
                    }
                }

                this.symbols.Dequeue();
            }

            while (this.symbols.Any() && this.symbols.Peek().Type == CSharpSymbolType.LeftBracket)
            {
                while (this.symbols.Peek().Type != CSharpSymbolType.RightBracket)
                {
                    step.ArrayExpression += this.symbols.Dequeue().Content;
                }

                step.ArrayExpression += "]";
                this.symbols.Dequeue();
            }

            return step;
        }

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