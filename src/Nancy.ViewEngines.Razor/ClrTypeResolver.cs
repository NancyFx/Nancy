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

        public Type Resolve(List<TSymbol> symbols)
        {
            this.symbols = new Queue<TSymbol>(symbols);

            var type = this.ResolveType();

            return type.Resolve(ResolveTypeByName);
        }

        protected abstract TypeNameParserStep ResolveType();

        protected string PopFullIdentifier()
        {
            var sb = new StringBuilder();
            while (this.symbols.Any())
            {
                var peekType = this.symbols.Peek().Type;

                if (peekType.Equals(this.keyword))
                {
                    return this.symbols.Dequeue().Content;
                }
                else if (peekType.Equals(this.identifier))
                {
                    sb.Append(this.symbols.Dequeue().Content);
                }
                else if (peekType.Equals(this.dot))
                {
                    this.symbols.Dequeue();
                    sb.Append(".");
                }
                else if (peekType.Equals(this.WhiteSpace))
                {
                    this.symbols.Dequeue();
                }
                else
                {
                    return sb.ToString();
                }
            }

            return sb.ToString();
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

        [DebuggerDisplay("{GenericTypeName}`{GenericArguments.Count}")]
        protected class TypeNameParserStep
        {
            public string GenericTypeName { get; private set; }
            public List<TypeNameParserStep> GenericArguments { get; private set; }

            public TypeNameParserStep(string name)
            {
                this.GenericTypeName = name;
                this.GenericArguments = new List<TypeNameParserStep>();
            }

            public Type Resolve(Func<string, Type> resolveType)
            {
                var effectiveArguments = this.GenericArguments.Where(x => x.GenericTypeName != string.Empty).ToArray();

                var genericType = resolveType(this.GenericTypeName + "`" + effectiveArguments.Length);

                if (effectiveArguments.Length == 0)
                {
                    return resolveType(this.GenericTypeName);
                }

                var genericArguments = effectiveArguments.Select(x => x.Resolve(resolveType)).ToArray();

                return genericType.MakeGenericType(genericArguments);
            }
        }
    }
}
