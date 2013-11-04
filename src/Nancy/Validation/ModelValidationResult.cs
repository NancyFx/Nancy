namespace Nancy.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The result of a validation.
    /// </summary>
    public class ModelValidationResult
    {
        /// <summary>
        /// Represents an instance of the <see cref="ModelValidationResult"/> type that will
        /// return <see langword="true"/> when <see cref="IsValid"/> is queried.
        /// </summary>
        public static readonly ModelValidationResult Valid = new ModelValidationResult();

        private ModelValidationResult()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public ModelValidationResult(IEnumerable<ModelValidationError> errors)
        {
            this.Errors = (errors == null) ?
                Enumerable.Empty<ModelValidationError>() :
                errors.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the <see cref="ModelValidationError"/> instances.</value>
        public IEnumerable<ModelValidationError> Errors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the validated instance is valid.
        /// </summary>
        /// <value><see langword="true"/> if the validated instance is valid; otherwise, <see langword="false"/>.</value>
        public bool IsValid
        {
            get { return !Errors.Any(); }
        }

        /// <summary>
        /// Creates a new ValidationResult with the added error.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>An <see cref="ModelValidationResult"/> instance.</returns>
        public ModelValidationResult AddError(string memberName, string errorMessage)
        {
            return new ModelValidationResult(this.Errors.Concat(new[] { new ModelValidationError(memberName, s => errorMessage) }));
        }

        /// <summary>
        /// Creates a new ValidationResult with the added error.
        /// </summary>
        /// <param name="memberNames">The member names.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>An <see cref="ModelValidationResult"/> instance.</returns>
        public ModelValidationResult AddError(IEnumerable<string> memberNames, string errorMessage)
        {
            return new ModelValidationResult(Errors.Concat(new[] { new ModelValidationError(memberNames, s => errorMessage) }));
        }
    }
}