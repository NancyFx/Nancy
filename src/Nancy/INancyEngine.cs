namespace Nancy
{
    using System;

    public interface INancyEngine
    {
        /// <summary>
        /// <para>
        /// Gets or sets the pre-request hook.
        /// </para>
        /// <para>
        /// The Pre-request hook is called prior to processing a request. If a hook returns
        /// a non-null response then processing is aborted and the response provided is
        /// returned.
        /// </para>
        /// </summary>
        Func<NancyContext, Response> PreRequestHook { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the post-requets hook.
        /// </para>
        /// <para>
        /// The post-request hook is called after a route is located and invoked. The post
        /// request hook can rewrite the response or add/remove items from the context
        /// </para>
        /// </summary>
        Action<NancyContext> PostRequestHook { get; set; }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        NancyContext HandleRequest(Request request);
    }
}