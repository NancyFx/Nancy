namespace Nancy
{
    /// <summary>
    /// An extension point for adding support for formatting response contents. No members should be added to this interface without good reason.
    /// </summary>
    /// <remarks>Extension methods to this interface should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
    public interface IResponseFormatter : IHideObjectMembers
    {
        string RootPath { get; }
    }
}