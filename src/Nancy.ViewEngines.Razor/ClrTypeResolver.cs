namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web.Razor.Tokenizer.Symbols;

    using Nancy.Bootstrapper;

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

    internal class CSharpClrTypeResolver : ClrTypeResolver<CSharpSymbolType, CSharpSymbol>
    {        
        public CSharpClrTypeResolver()
            :base(CSharpSymbolType.Identifier, CSharpSymbolType.Keyword, CSharpSymbolType.Dot, CSharpSymbolType.WhiteSpace)
        {            
        }

        protected override TypeNameParserStep ResolveType()
        {
            var identifier = this.PopFullIdentifier();

            var step = new TypeNameParserStep(identifier);

            if (this.symbols.Any() && this.symbols.Peek().Type == CSharpSymbolType.LessThan)
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

            return step;
        }

        protected override Type ResolvePrimitiveType(string typeName)
        {
            var primitives = new Dictionary<string, Type>
            {
                {"string", typeof (String)},
                {"int", typeof (int)}
            };

            return (primitives.ContainsKey(typeName) ? primitives[typeName] : null);
        }
    }

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
