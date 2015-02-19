namespace Nancy.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a model validation error.
    /// </summary>
    public class ModelValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member that the error describes.</param>
        /// <param name="errorMessage"></param>
        public ModelValidationError(string memberName, string errorMessage)
            : this(new[] { memberName }, errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationError"/> class.
        /// </summary>
        /// <param name="memberNames">The member names that the error describes.</param>
        /// <param name="errorMessage"></param>
        public ModelValidationError(IEnumerable<string> memberNames, string errorMessage)
        {
            this.MemberNames = memberNames;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the member names that are a part of the error.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the name of the members.</value>
        public IEnumerable<string> MemberNames { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Implicitly cast a validation error to a string.
        /// </summary>
        /// <param name="error">The <see cref="ModelValidationError"/> that should be cast.</param>
        /// <returns>A <see cref="string"/> containing the validation error description.</returns>
        public static implicit operator string(ModelValidationError error)
        {
            return error.ErrorMessage;
        }

        /// <summary>
        /// Returns the <see cref="ErrorMessage"/>.
        /// </summary>
        /// <returns>A string containing the error message.</returns>
        public override string ToString()
        {
            return this.ErrorMessage;
        }
    }
}