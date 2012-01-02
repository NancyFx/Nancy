namespace Nancy.Validation.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class StringLengthValidationRule : ModelValidationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthValidationRule"/> class.
        /// </summary>
        /// <param name="errorMessageFormatter">The error message formatter.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="minLength">Length of the min.</param>
        /// <param name="maxLength">Length of the max.</param>
        public StringLengthValidationRule(Func<string, string> errorMessageFormatter, IEnumerable<string> memberNames, int minLength, int maxLength)
            : base("StringLength", errorMessageFormatter, memberNames)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;
        }

        /// <summary>
        /// Gets the length of the min.
        /// </summary>
        /// <value>The length of the min.</value>
        public int MinLength { get; private set; }

        /// <summary>
        /// Gets the length of the max.
        /// </summary>
        /// <value>The length of the max.</value>
        public int MaxLength { get; private set; }
    }
}
