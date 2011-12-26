namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A validation error.
    /// </summary>
    public class ModelValidationError
    {
        private readonly Func<string, string> errorMessageFormatter;

        /// <summary>
        /// Gets the member names that are a part of the error.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the name of the members.</value>
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ModelValidationError(string memberName, Func<string, string> errorMessageFormatter)
            : this(new[] { memberName }, errorMessageFormatter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ModelValidationError(IEnumerable<string> memberNames, Func<string, string> errorMessageFormatter)
        {
            this.MemberNames = memberNames;
            this.errorMessageFormatter = errorMessageFormatter;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>The error message.</returns>
        public string GetMessage(string displayName)
        {
            return errorMessageFormatter(displayName);
        }
    }
}