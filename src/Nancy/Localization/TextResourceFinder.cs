namespace Nancy.Localization
{
    using System;
    using System.Dynamic;

    /// <summary>
    /// Returns text from an implemented ITextResource
    /// </summary>
    public class TextResourceFinder : DynamicObject
    {
        private readonly ITextResource textResource;
        private readonly NancyContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextResourceFinder"/> class.
        /// </summary>
        /// <param name="textResource">The <see cref="ITextResource"/> that should be used by the TextResourceFinder</param>
        /// <param name="context">The <see cref="NancyContext"/> that should be used by the TextResourceFinder</param>
        public TextResourceFinder(ITextResource textResource, NancyContext context)
        {
            this.textResource = textResource;
            this.context = context;
        }

        /// <summary>
        /// Gets the <see cref="ITextResource"/> that is being used to locate texts.
        /// </summary>
        /// <value>An <see cref="ITextResource"/> instance.</value>
        public ITextResource Resource
        {
            get { return this.textResource; }
        }

        /// <summary>
        /// Finds text resource
        /// </summary>
        /// <param name="binder">GetMemberBinder with dynamic text key</param>
        /// <param name="result">Text item</param>
        /// <returns>Returns a value or a non existing value from the <see cref="ITextResource"/> implementation</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result =
                new DynamicMemberChainer(binder.Name, this.context, this.textResource);

            return true;
        }

        /// <summary>
        /// Gets a translation based on the provided key.
        /// </summary>
        /// <param name="key">The key to look up the translation for.</param>
        public string this[string key]
        {
            get
            {
                return this.textResource[key, this.context];
            }
        }

        /// <summary>
        /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        public class DynamicMemberChainer : DynamicObject
        {
            private string memberName;
            private readonly NancyContext context;
            private readonly ITextResource textResource;

            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicMemberChainer"/> class.
            /// </summary>
            /// <param name="memberName">Name of the member.</param>
            /// <param name="context">The nancy context instance.</param>
            /// <param name="resource">The text resource instance.</param>
            public DynamicMemberChainer(string memberName, NancyContext context, ITextResource resource)
            {
                this.memberName = memberName;
                this.context = context;
                this.textResource = resource;
            }

            /// <summary>
            /// Gets the member name concatenated to binder name.
            /// </summary>
            /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
            /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
            /// </returns>
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                this.memberName =
                    string.Concat(this.memberName, ".", binder.Name);

                result = this;

                return true;
            }

            /// <summary>
            /// Attempts to convert provided member name and context to the text resource representation.
            /// </summary>
            /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Type returns the <see cref="T:System.String" /> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
            /// <param name="result">The result of the type conversion operation.</param>
            /// <returns>
            /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
            /// </returns>
            /// <exception cref="System.InvalidOperationException">Cannot cast dynamic member access to anything else than a string.</exception>
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                if (binder.ReturnType == typeof(string))
                {
                    result = this.textResource[this.memberName, this.context];
                    return true;
                }

                throw new InvalidOperationException("Cannot cast dynamic member access to anything else than a string.");
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return this.textResource[this.memberName, this.context];
            }
        }
    }
}
