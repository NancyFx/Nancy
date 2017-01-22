namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interactive diagnostic method.
    /// </summary>
    public class InteractiveDiagnosticMethod
    {
        /// <summary>
        /// Gets the parent diagnostic object.
        /// </summary>
        /// <value>The parent diagnostic object.</value>
        public object ParentDiagnosticObject { get; private set; }

        /// <summary>
        /// Gets the return type
        /// </summary>
        /// <value>The type of the method return type</value>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description of the method.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments for the method.</value>
        public IEnumerable<Tuple<string, Type>> Arguments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveDiagnosticMethod"/> class, with
        /// the provided <paramref name="parentDiagnostic"/>, <paramref name="returnType"/>, 
        /// <paramref name="methodName"/>, <paramref name="arguments"/> and <paramref name="description"/>.
        /// </summary>
        /// <param name="parentDiagnostic">The parent diagnostic.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="description">The description.</param>
        public InteractiveDiagnosticMethod(object parentDiagnostic, Type returnType, string methodName, IEnumerable<Tuple<string, Type>> arguments, string description)
        {
            this.ParentDiagnosticObject = parentDiagnostic;
            this.ReturnType = returnType;
            this.MethodName = methodName;
            this.Arguments = arguments;
            this.Description = description;
        }
    }
}