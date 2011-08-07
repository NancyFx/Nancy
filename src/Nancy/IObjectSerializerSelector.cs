namespace Nancy
{
    /// <summary>
    /// Allows setting of the serializer for session object storage
    /// </summary>
    public interface IObjectSerializerSelector : IHideObjectMembers
    {
        /// <summary>
        /// Using the specified serializer
        /// </summary>
        /// <param name="newSerializer">Serializer to use</param>
        void WithSerializer(IObjectSerializer newSerializer);
    }
}