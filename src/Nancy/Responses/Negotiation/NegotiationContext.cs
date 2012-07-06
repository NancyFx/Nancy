using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Gets or sets the model mappings for media ranges
        /// </summary>
        public IDictionary<MediaRange, Func<dynamic>> MediaRangeModelMappings { get; set; }

        /// <summary>
        /// Gets or sets the additional response headers required
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        public NegotiationContext()
        {
            this.PermissableMediaRanges = new List<MediaRange>(new[] { (MediaRange)"*/*" });
            this.MediaRangeModelMappings = new Dictionary<MediaRange, Func<dynamic>>();
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the correct model for the given media range
        /// </summary>
        /// <param name="range">Media range</param>
        /// <returns>Model object</returns>
        public dynamic GetModelForMediaRange(MediaRange range)
        {
            var matching =
                this.MediaRangeModelMappings.Any(
                    m => range.Type.Equals(m.Key.Type) && range.Subtype.Equals(m.Key.Subtype));

            return matching
                        ? this.MediaRangeModelMappings.First(m => range.Type.Equals(m.Key.Type) && range.Subtype.Equals(m.Key.Subtype))
                        : this.DefaultModel;
        }
    }
}