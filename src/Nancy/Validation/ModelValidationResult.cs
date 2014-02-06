namespace Nancy.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the result of a model validation.
    /// </summary>
    public class ModelValidationResult
    {
        /// <summary>
        /// Represents an instance of the <see cref="ModelValidationResult"/> type that will
        /// return <see langword="true"/> when <see cref="IsValid"/> is queried.
        /// </summary>
        public static readonly ModelValidationResult Valid = new ModelValidationResult();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The <see cref="ModelValidationError"/> instances that makes up the result.</param>
        public ModelValidationResult(IEnumerable<ModelValidationError> errors)
            : this(GetModelValidationErrorDictionary((errors ?? Enumerable.Empty<ModelValidationError>()).ToArray()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The <see cref="ModelValidationError"/> instances that makes up the result, grouped by member name.</param>
        public ModelValidationResult(IDictionary<string, IList<ModelValidationError>> errors)
        {
            this.Errors = errors;
        }

        private ModelValidationResult()
            : this(Enumerable.Empty<ModelValidationError>())
        {
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains <see cref="ModelValidationError"/> instances grouped by property name.</value>
        public IDictionary<string, IList<ModelValidationError>> Errors { get; set; }

        /// <summary>
        /// Gets a value indicating whether the validated instance is valid or not.
        /// </summary>
        /// <value><see langword="true"/> if the validated instance is valid; otherwise, <see langword="false"/>.</value>
        public bool IsValid
        {
            get { return !Errors.Keys.Any(); }
        }

        private static IDictionary<string, IList<ModelValidationError>> GetModelValidationErrorDictionary(ModelValidationError[] results)
        {
            var output =
                new Dictionary<string, IList<ModelValidationError>>(StringComparer.OrdinalIgnoreCase);

            if (results == null || !results.Any())
            {
                return output;
            }

            foreach (var result in results)
            {
                foreach (var name in result.MemberNames)
                {
                    if (!output.ContainsKey(name))
                    {
                        output.Add(name, new List<ModelValidationError>());
                    }

                    output[name].Add(result);
                }
            }

            return output;
        }
    }
}