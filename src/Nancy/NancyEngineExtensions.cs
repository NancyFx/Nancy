namespace Nancy
{
    using System;

    using Nancy.Helpers;

    public static class NancyEngineExtensions
    {
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        public static NancyContext HandleRequest(this INancyEngine nancyEngine, Request request)
        {
            return HandleRequest(nancyEngine, request, context => context);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest"></param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        public static NancyContext HandleRequest(this INancyEngine nancyEngine, Request request, Func<NancyContext, NancyContext> preRequest)
        {
            var task = nancyEngine.HandleRequest(request, preRequest);
            task.Wait();
            if (task.IsFaulted)
            {
                throw task.Exception ?? new Exception("Request task faulted");
            }
            return task.Result;
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public static void HandleRequest(
            this INancyEngine nancyEngine,
            Request request,
            Func<NancyContext, NancyContext> preRequest,
            Action<NancyContext> onComplete,
            Action<Exception> onError)
        {
            if (nancyEngine == null)
            {
                throw new ArgumentNullException("nancyEngine");
            }

            nancyEngine
                .HandleRequest(request, preRequest)
                .WhenCompleted(t => onComplete(t.Result), t => onError(t.Exception));

            //this.HandleRequest(request, null, onComplete, onError);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        public static void HandleRequest(
            this INancyEngine nancyEngine,
            Request request,
            Action<NancyContext> onComplete,
            Action<Exception> onError)
        {
            HandleRequest(nancyEngine, request, null, onComplete, onError);
        }
    }
}