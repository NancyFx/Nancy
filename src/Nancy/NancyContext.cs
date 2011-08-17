using Nancy.Security;

namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Nancy context.
    /// </summary>
    public sealed class NancyContext : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyContext"/> class.
        /// </summary>
        public NancyContext()
        {
            this.Items = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the dictionary for storage of per-request items. Disposable items will be disposed when the context is.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the parameters for the resolved route 
        /// </summary>
        public dynamic Parameters { get; set; }

        /// <summary>
        /// Gets or sets the incoming request
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// Gets or sets the outgoing response
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public IUserIdentity CurrentUser { get; set; }

        /// <summary>
        /// Disposes any disposable items in the <see cref="Items"/> dictionary.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposableItem in this.Items.Values.OfType<IDisposable>())
            {
                disposableItem.Dispose();
            }

            this.Items.Clear();
        }
    }
}
