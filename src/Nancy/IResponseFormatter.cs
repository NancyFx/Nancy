namespace Nancy
{
    /// <summary>
<<<<<<< HEAD
    /// An extension point for adding support for formatting response contents. No members should be added to this interface.
    /// </summary>
    /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
    /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
    public interface IResponseFormatter
=======
    /// An extension point for adding support for formatting response contents. No members should be added to this interface without good reason.
    /// </summary>
    /// <remarks>Extension methods to this interface should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
    public interface IResponseFormatter : IHideObjectMembers
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
    {
        string RootPath { get; }
    }
}