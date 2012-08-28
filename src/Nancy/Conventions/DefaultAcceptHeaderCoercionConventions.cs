using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.Conventions
{
    public class DefaultAcceptHeaderCoercionConventions : IConvention
    {
        private static readonly IEnumerable<Tuple<string, decimal>> DEFAULT_ACCEPT = new[] { Tuple.Create("text/html", 1.0m), Tuple.Create("*/*", 0.9m) };

        private static readonly string[] BROKEN_BROWSERS = new[] {"MSIE 8", "MSIE 7", "MSIE 6", "AppleWebKit"};

        public void Initialise(NancyConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);
        }

        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The accept header coercion conventions cannot be null.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private void ConfigureDefaultConventions(NancyConventions conventions)
        {
            conventions.AcceptHeaderCoercionConventions = new List<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>>(2)
                                                              {
                                                                  CoerceStupidBrowsers, 
                                                                  BoostHtml,
                                                                  CoerceBlankAcceptHeader,
                                                              };
        }

        /// <summary>
        /// Adds a default accept header if there isn't one.
        /// </summary>
        /// <param name="currentAcceptHeaders">Current headers</param>
        /// <param name="context">Context</param>
        /// <returns>Modified headers or original if no modification required</returns>
        private IEnumerable<Tuple<string, decimal>> CoerceBlankAcceptHeader(IEnumerable<Tuple<string, decimal>> currentAcceptHeaders, NancyContext context)
        {
            var current = currentAcceptHeaders as Tuple<string, decimal>[] ?? currentAcceptHeaders.ToArray();

            return !current.Any() ? DEFAULT_ACCEPT : current;
        }

        /// <summary>
        /// Replaces the accept header of stupid browsers that request XML instead
        /// of HTML.
        /// </summary>
        /// <param name="currentAcceptHeaders">Current headers</param>
        /// <param name="context">Context</param>
        /// <returns>Modified headers or original if no modification required</returns>
        private IEnumerable<Tuple<string, decimal>> CoerceStupidBrowsers(IEnumerable<Tuple<string, decimal>> currentAcceptHeaders, NancyContext context)
        {
            var current = currentAcceptHeaders as Tuple<string, decimal>[] ?? currentAcceptHeaders.ToArray();

            return this.IsStupidBrowser(current, context) ? DEFAULT_ACCEPT : current;
        }

        /// <summary>
        /// Boosts the priority of HTML for browsers that ask for xml and html with the
        /// same priority.
        /// </summary>
        /// <param name="currentAcceptHeaders">Current headers</param>
        /// <param name="context">Context</param>
        /// <returns>Modified headers or original if no modification required</returns>
        private IEnumerable<Tuple<string, decimal>> BoostHtml(IEnumerable<Tuple<string, decimal>> currentAcceptHeaders, NancyContext context)
        {
            var current = currentAcceptHeaders as Tuple<string, decimal>[] ?? currentAcceptHeaders.ToArray();

            var html = current.FirstOrDefault(h => h.Item1 == "text/html" && h.Item2 < 1.0m);

            if (html == null)
            {
                return current;
            }

            var index = Array.IndexOf(current, html);
            if (index == -1)
            {
                return current;
            }

            current[index] = Tuple.Create("text/html", html.Item2 + 0.2m);

            return current;
        }

        private bool IsStupidBrowser(Tuple<string, decimal>[] current, NancyContext context)
        {
            // If there's one or less accept headers then we can't be a stupid
            // browser so just bail out early
            if (current.Length <= 1)
            {
                return false;
            }

            var maxScore = current.First().Item2;

            if (this.IsPotentiallyBrokenBrowser(context.Request.Headers.UserAgent) 
                  && !current.Any(h => h.Item2 == maxScore && string.Equals("text/html", h.Item1)))
            {
                return true;
            }

            return false;
        }

        private bool IsPotentiallyBrokenBrowser(string userAgent)
        {
            return BROKEN_BROWSERS.Any(userAgent.Contains);
        }
    }
}