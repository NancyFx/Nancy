namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// An extension point for adding support for formatting response contents. No members should be added to this interface without good reason.
    /// </summary>
    /// <remarks>Extension methods to this interface should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
    public interface IResponseFormatter : IHideObjectMembers
    {
        /// <summary>
        /// Gets all serializers currently registered
        /// </summary>
        IEnumerable<ISerializer> Serializers { get; }

        /// <summary>
        /// Gets the context for which the response is being formatted.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> intance.</value>
        NancyContext Context { get; }

        /// <summary>
        /// Gets the root path of the application.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path.</value>
        string RootPath { get; }
    }
}