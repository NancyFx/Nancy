namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The result of a validation.
    /// </summary>
    public class ValidationResult
    {
        public static readonly ValidationResult Valid = new ValidationResult();

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IEnumerable<ValidationError> Errors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the validated instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the validated instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return !Errors.Any(); }
        }

        private ValidationResult()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public ValidationResult(IEnumerable<ValidationError> errors)
        {
            Errors = errors == null
                ? new List<ValidationError>().AsReadOnly()
                : errors.ToList().AsReadOnly();
        }

        /// <summary>
        /// Creates a new ValidationResult with the added error.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public ValidationResult AddError(string memberName, string errorMessage)
        {
            return new ValidationResult(Errors.Concat(new[] { new ValidationError(memberName, s => errorMessage) }));
        }

        /// <summary>
        /// Creates a new ValidationResult with the added error.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public ValidationResult AddError(IEnumerable<string> memberNames, string errorMessage)
        {
            return new ValidationResult(Errors.Concat(new[] { new ValidationError(memberNames, s => errorMessage) }));
        }
    }
}