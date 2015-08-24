namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Razor.Tokenizer.Symbols;

    /// <summary>
    /// Resolves VB.NET type name to CLR type
    /// </summary>
    internal class VisualBasicClrTypeResolver : ClrTypeResolver<VBSymbolType, VBSymbol>
    {
        public VisualBasicClrTypeResolver()
            : base(VBSymbolType.Identifier, VBSymbolType.Keyword, VBSymbolType.Dot, VBSymbolType.WhiteSpace)
        {
        }

        protected override TypeNameParserStep ResolveType()
        {
            var identifier = this.PopFullIdentifier();

            var step = new TypeNameParserStep(identifier);

            if (this.symbols.Any() && this.symbols.Peek().Type == VBSymbolType.LeftParenthesis)
            {
                var next = this.symbols.ElementAt(1);
                if (next.Type == VBSymbolType.Keyword && next.Keyword == VBKeyword.Of)
                {
                    this.symbols.Dequeue();
                    this.symbols.Dequeue();

                    while (this.symbols.Peek().Type != VBSymbolType.RightParenthesis)
                    {
                        step.GenericArguments.Add(this.ResolveType());

                        while (this.symbols.Peek().Type.Equals(this.WhiteSpace) || this.symbols.Peek().Type == VBSymbolType.Comma)
                        {
                            this.symbols.Dequeue();
                        }
                    }

                    this.symbols.Dequeue();
                }
            }

            while (this.symbols.Any() && this.symbols.Peek().Type == VBSymbolType.LeftParenthesis)
            {
                this.symbols.Dequeue();

                step.ArrayExpression += "[";
                while (this.symbols.Peek().Type != VBSymbolType.RightParenthesis)
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
                {"String", typeof (string)},
                {"Integer", typeof (int)},
                {"UInteger", typeof (uint)},
                {"Boolean", typeof (bool)},
                {"Byte", typeof (byte)},
                {"Char", typeof (char)},
                {"Decimal", typeof (decimal)},
                {"Double", typeof (double)},
                {"Short", typeof (Int16)},
                {"UShort", typeof (UInt16)},
                {"Long", typeof (Int64)},
                {"ULong", typeof (UInt64)},
                {"Object", typeof (object)},
                {"SByte", typeof (SByte)},
                {"Single", typeof (Single)},                
            };

            return (primitives.ContainsKey(typeName) ? primitives[typeName] : null);
        }
    }
}