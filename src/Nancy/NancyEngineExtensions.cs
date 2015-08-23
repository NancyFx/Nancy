namespace Nancy
{
    using System;
    using System.Threading;

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
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        public static NancyContext HandleRequest(this INancyEngine nancyEngine, Request request, Func<NancyContext, NancyContext> preRequest)
        {
            var task = nancyEngine.HandleRequest(request, preRequest, CancellationToken.None);

            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                throw ex.FlattenInnerExceptions();
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
        /// <param name="onError">Delegate to call when any errors occur</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static void HandleRequest(
            this INancyEngine nancyEngine,
            Request request,
            Func<NancyContext, NancyContext> preRequest,
            Action<NancyContext> onComplete,
            Action<Exception> onError,
            CancellationToken cancellationToken)
        {
            if (nancyEngine == null)
            {
                throw new ArgumentNullException("nancyEngine");
            }

            nancyEngine
                .HandleRequest(request, preRequest, cancellationToken)
                .WhenCompleted(t => onComplete(t.Result), t => onError(t.Exception));

            //this.HandleRequest(request, null, onComplete, onError);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Delegate to call when any errors occur</param>
        public static void HandleRequest(
            this INancyEngine nancyEngine,
            Request request,
            Action<NancyContext> onComplete,
            Action<Exception> onError)
        {
            HandleRequest(nancyEngine, request, null, onComplete, onError, CancellationToken.None);
        }
    }
}