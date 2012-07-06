using System;
using System.Collections.Generic;

namespace Nancy.Responses.Negotiation
{
    /// <summary>
    /// Context for content negotiation
    /// </summary>
    public class NegotiationContext
    {
        /// <summary>
        /// Gets or sets the view name if one is required
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// The module path of the <see cref="NancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the module path.</value>
        public string ModulePath { get; set; }

        /// <summary>
        /// The name of the <see cref="NancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets allowed media ranges
        /// </summary>
        public IList<MediaRange> PermissableMediaRanges { get; set; }

        /// <summary>
        /// Gets or sets the default model that will be used if a
        /// content type specific model is not speficied
        /// </summary>
        public dynamic DefaultModel { get; set; }

        /// <summary>
        /// Gets or sets the model mapping for specific content types
        /// </summary>
        public IDictionary<string, Func<object>> ContentTypeModelMappings { get; set; }

        /// <summary>
        /// Gets or sets the additional response headers required
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        public NegotiationContext()
        {
            this.PermissableMediaRanges = new List<MediaRange>(new[] { (MediaRange)"*/*" });
            this.Headers = new Dictionary<string, string>();
        }
    }
}