namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Razor.Tokenizer.Symbols;

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
                this.symbols.Dequeue();

                if (this.symbols.Peek().Type == VBSymbolType.Keyword && this.symbols.Peek().Keyword == VBKeyword.Of)
                {
                    this.symbols.Dequeue();

                    while (this.symbols.Peek().Type != VBSymbolType.RightParenthesis)
                    {
                        step.GenericArguments.Add(this.ResolveType());

                        while (this.symbols.Peek().Type.Equals(this.WhiteSpace) || this.symbols.Peek().Type == VBSymbolType.Comma)
                        {
                            this.symbols.Dequeue();
                        }
                    }
                }

                this.symbols.Dequeue();
            }

            return step;
        }

        protected override Type ResolvePrimitiveType(string typeName)
        {
            var primitives = new Dictionary<string, Type>
            {
                {"String", typeof (String)},
                {"Integer", typeof (int)}
            };

            return (primitives.ContainsKey(typeName) ? primitives[typeName] : null);
        }
    }
}