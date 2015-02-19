namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Cookies;

    using Nancy.Extensions;

    /// <summary>
    /// Context for content negotiation.
    /// </summary>
    public class NegotiationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NegotiationContext"/> class.
        /// </summary>
        public NegotiationContext()
        {
            this.Cookies = new List<INancyCookie>();
            this.PermissableMediaRanges = new List<MediaRange>(new[] { (MediaRange)"*/*" });
            this.MediaRangeModelMappings = new Dictionary<MediaRange, Func<dynamic>>();
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets additional cookies to assign to the response.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> of <see cref="INancyCookie"/> instances.</value>
        public IList<INancyCookie> Cookies { get; set; }

        /// <summary>
        /// Gets or sets the default model that will be used if a content type specific model is not specified.
        /// </summary>
        /// <value>The default model instance.</value>
        public dynamic DefaultModel { get; set; }

        /// <summary>
        /// Gets or sets the additional response headers required.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> containing the headers.</value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the model mappings for media ranges.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> containing the media range model mappings.</value>
        public IDictionary<MediaRange, Func<dynamic>> MediaRangeModelMappings { get; set; }

        /// <summary>
        /// The name of the <see cref="INancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// The module path of the <see cref="INancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the module path.</value>
        public string ModulePath { get; set; }

        /// <summary>
        /// Gets or sets allowed media ranges.
        /// </summary>
        /// <value>A list of the allowed media ranges.</value>
        public IList<MediaRange> PermissableMediaRanges { get; set; }

        /// <summary>
        /// Gets or sets the status code of the response.
        /// </summary>
        /// <value>A <see cref="HttpStatusCode"/> value.</value>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code returned to the client.
        /// </summary>
        /// <value>The HTTP status code description.</value>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the view name if one is required.
        /// </summary>
        /// <value>The name of the view that should be rendered.</value>
        public string ViewName { get; set; }

        /// <summary>
        /// Gets the correct model for the given media range
        /// </summary>
        /// <param name="mediaRange">The <see cref="MediaRange"/> to get the model for.</param>
        /// <returns>The model for the provided <paramref name="mediaRange"/> if it has been mapped, otherwise the <see cref="DefaultModel"/> will be returned.</returns>
        public dynamic GetModelForMediaRange(MediaRange mediaRange)
        {
            var matching = this.MediaRangeModelMappings.Any(m => mediaRange.Matches(m.Key));

            return matching ?
                this.MediaRangeModelMappings.First(m => mediaRange.Matches(m.Key)).Value.Invoke() :
                this.DefaultModel;
        }

        internal void SetModule(INancyModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            this.ModuleName = module.GetModuleName();
            this.ModulePath = module.ModulePath;
        }
    }
}