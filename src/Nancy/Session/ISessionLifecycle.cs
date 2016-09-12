namespace Nancy.Session
{
    using Nancy.Bootstrapper;

    /// <summary>
    /// <para>Defines the behaviour in each stage of the lifecycle of a NancySession.</para>
    /// <para>Initialise->OnStart->LoadSessionProvider->OnEnd->OnStart ... </para>
    /// </summary>
    public interface ISessionLifecycle
    {
        /// <summary>
        /// Initialisation step called immediately after creation. (Called only once)
        /// </summary>
        /// <param name="pipelines">pipelines</param>
        void Initialise(IPipelines pipelines);
        
        /// <summary>
        /// The session provider will be passed in immediately after it is created. The lifecycle will need to perform initialisation for the session provider
        /// </summary>
        /// <param name="provider">the loaded session provider</param>
        void LoadSessionProvider(SessionProvider provider);
        /// <summary>
        /// On start of pipeline, this is where initialisation of session should happen
        /// </summary>
        /// <param name="ctx">Nancy Context</param>
        /// <returns>Response after initialising session</returns>
        Response OnStart(NancyContext ctx);
        /// <summary>
        /// At the end of pipeline.
        /// </summary>
        /// <param name="ctx"></param>
        void OnEnd(NancyContext ctx);
    }
}
