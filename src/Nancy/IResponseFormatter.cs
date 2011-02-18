namespace Nancy
{
    /// <summary>
    /// An extension point for adding support for formatting response contents. No members should be added to this interface.
    /// </summary>
    /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
    /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
    public interface IResponseFormatter
    {
    }
}