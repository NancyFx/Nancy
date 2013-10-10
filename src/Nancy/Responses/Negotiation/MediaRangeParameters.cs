namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides strongly-typed access to media range parameters.
    /// </summary>
    public class MediaRangeParameters : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRangeParameters"/> class.
        /// </summary>
        public MediaRangeParameters()
        {
            this.parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRangeParameters"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public MediaRangeParameters(IDictionary<string, string> parameters)
        {
            this.parameters = new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the names of the available parameters.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> containing the names of the parameters.</value>
        public IEnumerable<string> Keys
        {
            get { return this.parameters.Keys; }
        }

        /// <summary>
        /// Gets all the parameters values.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains all the parameters values.</value>
        public IEnumerable<string> Values
        {
            get { return this.parameters.Values; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        /// <summary>
        /// Whether or not a set of media range parameters matches another, regardless of order
        /// </summary>
        /// <param name="other">Other media range parameters</param>
        /// <returns>True if matching, false if not</returns>
        public bool Matches(MediaRangeParameters other)
        {
            return this.parameters.OrderBy(p => p.Key).SequenceEqual(other.parameters.OrderBy(p => p.Key));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the value for the parameter identified by the <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The name of the parameter to return the value for.</param>
        /// <returns>The value for the parameter. If the parameter is not defined then null is returned.</returns>
        public string this[string name]
        {
            get
            {
                return (this.parameters.ContainsKey(name)) ? this.parameters[name] : null;
            }
        }

        public static implicit operator string(MediaRangeParameters mediaRangeParameters)
        {
            return string.Join(";", mediaRangeParameters.parameters.Select(p => p.Key + "=" + p.Value));
        }

        /// <summary>
        /// Creates a MediaRangeParameters collection from a "a=1,b=2" string
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MediaRangeParameters FromString(string parameters)
        {
            var dictionary = parameters.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0].Trim(), split => split[1].Trim());

            return new MediaRangeParameters(dictionary);
        }

        public override string ToString()
        {
            return this;
        }
    }
}