namespace Nancy.Sessions
{
    /// <summary>
    /// Defines the interface for storing and retrieving session information
    /// </summary>
    public interface ISessionStore
    {
        void Save(ISession session, Response response);
        ISession Load(Request request);
    }
}