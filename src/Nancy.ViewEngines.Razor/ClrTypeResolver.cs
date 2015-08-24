namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web.Razor.Tokenizer.Symbols;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Converts language-specific type name into CRL Type
    /// </summary>
    /// <typeparam name="TSymbolType">Symbol type enum</typeparam>
    /// <typeparam name="TSymbol">Symbol class</typeparam>
    internal abstract class ClrTypeResolver<TSymbolType, TSymbol>
        where TSymbol : SymbolBase<TSymbolType>
    {
        private readonly TSymbolType identifier;
        private readonly TSymbolType keyword;
        private readonly TSymbolType dot;
        protected readonly TSymbolType WhiteSpace;
        protected Queue<TSymbol> symbols;

        protected ClrTypeResolver(TSymbolType identifier, TSymbolType keyword, TSymbolType dot, TSymbolType whiteSpace)
        {
            this.identifier = identifier;
            this.keyword = keyword;
            this.dot = dot;
            this.WhiteSpace = whiteSpace;
        }

        /// <summary>
        /// Parses given list of symbols in order to get CLR type
        /// </summary>
        /// <param name="symbols">List of symbols</param>
        /// <returns>CLR type</returns>
        public Type Resolve(List<TSymbol> symbols)
        {
            this.symbols = new Queue<TSymbol>(symbols);

            var type = this.ResolveType();

            return type.Resolve(ResolveTypeByName);
        }

        protected abstract TypeNameParserStep ResolveType();

        /// <summary>
        /// Pops full identifier from symbols queue. 
        /// This method recognizes keywords (like int) and multipart identifiers (like System.Object)
        /// </summary>
        /// <returns>Popped identifier</returns>
        protected string PopFullIdentifier()
        {
            var builder = new StringBuilder();
            while (this.symbols.Any())
            {
                var peekType = this.symbols.Peek().Type;

                if (peekType.Equals(this.keyword))
                {
                    return this.symbols.Dequeue().Content;
                }
                else if (peekType.Equals(this.identifier))
                {
                    builder.Append(this.symbols.Dequeue().Content);
                }
                else if (peekType.Equals(this.dot))
                {
                    this.symbols.Dequeue();
                    builder.Append(".");
                }
                else if (peekType.Equals(this.WhiteSpace))
                {
                    this.symbols.Dequeue();
                }
                else
                {
                    return builder.ToString();
                }
            }

            return builder.ToString();
        }

        private Type ResolveTypeByName(string typeName)
        {
            return Type.GetType(typeName)
                   ?? ResolvePrimitiveType(typeName)
                   ?? AppDomainAssemblyTypeScanner.Types.FirstOrDefault(t => t.FullName == typeName)
                   ?? AppDomainAssemblyTypeScanner.Types.FirstOrDefault(t => t.Name == typeName)
                ;
        }

        protected abstract Type ResolvePrimitiveType(string typeName);

        /// <summary>
        /// Represents partially constructed type name. After construction it is possible to add generic arguments or array expression
        /// </summary>
        [DebuggerDisplay("{GenericTypeName}`{GenericArguments.Count}")]
        protected class TypeNameParserStep
        {
            public string GenericTypeName { get; set; }
            public List<TypeNameParserStep> GenericArguments { get; private set; }
            public string ArrayExpression { get; set; }

            public TypeNameParserStep(string name)
            {
                this.GenericTypeName = name;
                this.GenericArguments = new List<TypeNameParserStep>();
                this.ArrayExpression = string.Empty;
            }

            public Type Resolve(Func<string, Type> resolveType)
            {
                var effectiveArguments = this.GenericArguments.Where(x => x.GenericTypeName != string.Empty).ToArray();

                var genericType = resolveType(string.Format("{0}`{1}", this.GenericTypeName, effectiveArguments.Length));

                Type resultType = null;

                if (effectiveArguments.Length == 0)
                {
                    resultType = resolveType(this.GenericTypeName);
                }              
                else
                {
                    var genericArguments = effectiveArguments.Select(x => x.Resolve(resolveType)).ToArray();

                    resultType = genericType.MakeGenericType(genericArguments);
                }

                if (this.ArrayExpression != "")
                {
                    resultType = resolveType(resultType.FullName + this.ArrayExpression);
                }

                return resultType;
            }
        }
    }
}
