namespace Nancy.Validation.Rules
{
    /// <summary>
    /// Specifies the validation comparison operators used by the <see cref="ComparisonValidationRule"/> type.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// A comparison for greater than.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// A comparison for greater than or equal to.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// A comparison for less than.
        /// </summary>
        LessThan,

        /// <summary>
        /// A comparison for less than or equal to.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// A comparison for equality.
        /// </summary>
        Equal,

        /// <summary>
        /// A comparison for inequality.
        /// </summary>
        NotEqual
    }
}