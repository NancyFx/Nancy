namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a model validation error.
    /// </summary>
    public class ModelValidationError
    {
        private readonly Func<IEnumerable<string>, string> errorMessageFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member that the error describes.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ModelValidationError(string memberName, Func<IEnumerable<string>,string> errorMessageFormatter)
            : this(new[] { memberName }, errorMessageFormatter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberNames">The member names that the error describes.</param>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        public ModelValidationError(IEnumerable<string> memberNames, Func<IEnumerable<string>, string> errorMessageFormatter)
        {
            this.MemberNames = memberNames;
            this.errorMessageFormatter = errorMessageFormatter;
        }

        /// <summary>
        /// Gets the member names that are a part of the error.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the name of the members.</value>
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the validation error description.</returns>
        public string GetMessage()
        {
            return this.errorMessageFormatter(this.MemberNames);
        }

        /// <summary>
        /// Implictly cast a validation error to a string.
        /// </summary>
        /// <param name="error">The <see cref="ModelValidationError"/> that should be cast.</param>
        /// <returns>A <see cref="string"/> containing the validation error description.</returns>
        public static implicit operator string(ModelValidationError error)
        {
            return error.GetMessage();
        }
    }
}