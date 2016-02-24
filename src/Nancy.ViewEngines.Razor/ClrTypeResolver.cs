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
        private readonly RazorAssemblyProvider razorAssemblyProvider;

        private readonly TSymbolType identifier;
        private readonly TSymbolType keyword;
        private readonly TSymbolType dot;
        private readonly TSymbolType whiteSpace;
        private readonly TSymbolType arrayBegin;
        private readonly TSymbolType arrayEnd;

        /// <summary>
        /// Gets remaining symbols that need to be parsed
        /// </summary>
        protected Queue<TSymbol> Symbols { get; private set; }

        /// <summary>
        /// Initializes new instance of ClrTypeResolver class.
        /// Provided parameters are used to recognized specific symbols in particular language
        /// </summary>
        /// <param name="razorAssemblyProvider">An <see cref="RazorAssemblyProvider"/> used to resolve model types from the available assemblies.</param>
        /// <param name="identifier">Symbol type for identifier</param>
        /// <param name="keyword">Symbol type for keyword</param>
        /// <param name="dot">Symbol type for dot ('.')</param>
        /// <param name="whiteSpace">Symbol type for whitespace</param>
        /// <param name="arrayBegin">Type of symbol that begins array</param>
        /// <param name="arrayEnd">Type of symbol that ends array</param>
        protected ClrTypeResolver(RazorAssemblyProvider razorAssemblyProvider, TSymbolType identifier, TSymbolType keyword, TSymbolType dot, TSymbolType whiteSpace, TSymbolType arrayBegin, TSymbolType arrayEnd)
        {
            this.razorAssemblyProvider = razorAssemblyProvider;
            this.identifier = identifier;
            this.keyword = keyword;
            this.dot = dot;
            this.whiteSpace = whiteSpace;
            this.arrayBegin = arrayBegin;
            this.arrayEnd = arrayEnd;
        }

        /// <summary>
        /// Parses given list of symbols in order to get CLR type
        /// </summary>
        /// <param name="symbols">List of symbols</param>
        /// <returns>CLR type</returns>
        public Type Resolve(List<TSymbol> symbols)
        {
            this.Symbols = new Queue<TSymbol>(symbols);

            var type = this.ResolveType();

            return type.Resolve(ResolveTypeByName);
        }

        /// <summary>
        /// Dequeues symbols until first symbol of first generic argument
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected abstract bool MoveToGenericArguments();

        /// <summary>
        /// Dequeues symbols representing separator between generic arguments
        /// </summary>
        protected abstract void MoveToNextGenericArgument();

        /// <summary>
        /// Dequeues symbols representing end of generic arguments
        /// </summary>
        /// <returns>Returns true if move was successful</returns>
        protected abstract bool MoveOutOfGenericArguments();

        /// <summary>
        /// Gets CLR from name (keyword) used by specific language
        /// </summary>
        /// <param name="typeName">Type name to resolve</param>
        /// <returns>CLR type</returns>
        protected abstract Type ResolvePrimitiveType(string typeName);

        private TypeNameParserStep ResolveType()
        {
            var identifier = this.PopFullIdentifier();

            var step = new TypeNameParserStep(identifier);

            if (!this.Symbols.Any())
            {
                return step;
            }

            step.GenericArguments.AddRange(this.ReadGenericArguments());

            step.ArrayExpression = this.ReadArrayExpression();

            return step;
        }

        private List<TypeNameParserStep> ReadGenericArguments()
        {
            var genericArgs = new List<TypeNameParserStep>();

            if (this.MoveToGenericArguments())
            {
                while (!MoveOutOfGenericArguments())
                {
                    genericArgs.Add(this.ResolveType());

                    this.MoveToNextGenericArgument();
                }
            }

            return genericArgs;
        }

        private string PopFullIdentifier()
        {
            var builder = new StringBuilder();

            while (this.Symbols.Any())
            {
                var peekType = this.Symbols.Peek().Type;

                if (peekType.Equals(this.keyword))
                {
                    return this.Symbols.Dequeue().Content;
                }
                else if (peekType.Equals(this.identifier))
                {
                    builder.Append(this.Symbols.Dequeue().Content);
                }
                else if (peekType.Equals(this.dot))
                {
                    this.Symbols.Dequeue();
                    builder.Append(".");
                }
                else if (peekType.Equals(this.whiteSpace))
                {
                    this.Symbols.Dequeue();
                }
                else
                {
                    return builder.ToString();
                }
            }

            return builder.ToString();
        }

        private string ReadArrayExpression()
        {
            var arrayExpr = "";

            while (this.Symbols.Any() && this.Symbols.Peek().Type.Equals(this.arrayBegin))
            {
                this.Symbols.Dequeue();

                arrayExpr += "[";

                while (!(this.Symbols.Peek().Type.Equals(this.arrayEnd)))
                {
                    arrayExpr += this.Symbols.Dequeue().Content;
                }

                arrayExpr += "]";
                this.Symbols.Dequeue();
            }

            return arrayExpr;
        }

        private Type ResolveTypeByName(string typeName)
        {
            return Type.GetType(typeName)
                   ?? this.ResolvePrimitiveType(typeName)
                   ?? this.ResolveTypeFromAssemblyCatalog(typeName);
        }

        private Type ResolveTypeFromAssemblyCatalog(string typeName)
        {
            return this.razorAssemblyProvider.GetAssemblies().Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type != null);
        }

        [DebuggerDisplay("{GenericTypeName}`{GenericArguments.Count}")]
        private class TypeNameParserStep
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

            /// <summary>
            /// Resolves CLR type that is represented by this instance
            /// </summary>
            /// <param name="resolveType">Function that allows resolving any simple (like int or MyNamespace.SuperClass) type name to CLR type</param>
            /// <returns>Resolved CLR type</returns>
            public Type Resolve(Func<string, Type> resolveType)
            {
                var effectiveArguments = this.GenericArguments.Where(x => x.GenericTypeName != string.Empty).ToArray();

                Type resultType = null;

                if (effectiveArguments.Length == 0)
                {
                    resultType = resolveType(this.GenericTypeName);
                }
                else
                {
                    var genericArguments = effectiveArguments.Select(x => x.Resolve(resolveType)).ToArray();

                    var genericType = resolveType(string.Format("{0}`{1}", this.GenericTypeName, effectiveArguments.Length));

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
