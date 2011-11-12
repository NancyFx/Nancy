namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A validation error.
    /// </summary>
    public class ValidationError
    {
        private readonly Func<string, string> errorMessageFormatter;

        /// <summary>
        /// Gets the member names that are a part of the error.
        /// </summary>
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ValidationError(string memberName, Func<string, string> errorMessageFormatter)
            : this(new[] { memberName }, errorMessageFormatter)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ValidationError(IEnumerable<string> memberNames, Func<string, string> errorMessageFormatter)
        {
            MemberNames = memberNames;
            this.errorMessageFormatter = errorMessageFormatter;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public string GetMessage(string displayName)
        {
            return errorMessageFormatter(displayName);
        }
    }
}