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
        public VisualBasicClrTypeResolver(IAssemblyCatalog catalog)
            : base(catalog, VBSymbolType.Identifier, VBSymbolType.Keyword, VBSymbolType.Dot, VBSymbolType.WhiteSpace, VBSymbolType.LeftParenthesis, VBSymbolType.RightParenthesis)
        {
        }

        /// <summary>
        /// Dequeues symbol ')' representing end of generic arguments
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected override bool MoveOutOfGenericArguments()
        {
            if (this.Symbols.Peek().Type == VBSymbolType.RightParenthesis)
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
            while (this.Symbols.Peek().Type == VBSymbolType.WhiteSpace || this.Symbols.Peek().Type == VBSymbolType.Comma)
            {
                this.Symbols.Dequeue();
            }
        }

        /// <summary>
        /// Dequeues symbols '(Of' representing begin of generic arguments
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected override bool MoveToGenericArguments()
        {
            if (this.Symbols.Peek().Type != VBSymbolType.LeftParenthesis)
            {
                return false;
            }

            var next = this.Symbols.ElementAt(1);

            if (next.Type != VBSymbolType.Keyword || next.Keyword != VBKeyword.Of)
            {
                return false;
            }

            this.Symbols.Dequeue();
            this.Symbols.Dequeue();

            return true;
        }

        /// <summary>
        /// Gets CLR from name (keyword) used by VB.NET
        /// </summary>
        /// <param name="typeName">Type name to resolve</param>
        /// <returns>CLR type</returns>
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