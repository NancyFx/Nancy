namespace Nancy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the RFC 5899 Link HTTP header.
    /// </summary>
    public class HttpLinkHeader : IDictionary<HttpLinkRelation, IEnumerable<HttpLink>>
    {
        private readonly IDictionary<HttpLinkRelation, IEnumerable<HttpLink>> linkDictionary;


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLinkHeader"/> class.
        /// </summary>
        /// <param name="links">The links.</param>
        public HttpLinkHeader(params HttpLink[] links)
        {
            this.linkDictionary = new Dictionary<HttpLinkRelation, IEnumerable<HttpLink>>();

            if (links == null || !links.Any())
            {
                return;
            }

            foreach (var link in links)
            {
                this.Add(link);
            }
        }


        /// <summary>
        /// Gets or sets the element with the specified link <paramref name="relation"/> name.
        /// </summary>
        /// <value>
        /// The <see cref="HttpLink"/>.
        /// </value>
        /// <param name="relation">The relation.</param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="relation" /> is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public IEnumerable<HttpLink> this[string relation]
        {
            get { return this[HttpLinkRelation.Parse(relation)]; }
            set { this[HttpLinkRelation.Parse(relation)] = value; }
        }

        /// <summary>
        /// Gets or sets the element with the specified link <paramref name="relation" /> name.
        /// </summary>
        /// <value>
        /// The <see cref="IEnumerable{HttpLink}"/>.
        /// </value>
        /// <param name="relation">The relation.</param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="relation" /> is not found.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public IEnumerable<HttpLink> this[HttpLinkRelation relation]
        {
            get
            {
                if (!this.linkDictionary.ContainsKey(relation))
                {
                    throw new KeyNotFoundException($"The link relation name '{relation}' was not found in the HTTP Link Header dictionary.");
                }

                return this.linkDictionary[relation];
            }
            set
            {
                if (!this.linkDictionary.ContainsKey(relation))
                {
                    this.linkDictionary.Add(relation, new List<HttpLink>(value));
                }
                else
                {
                    this.linkDictionary[relation] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///   </returns>
        public int Count => this.linkDictionary.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        ///   </returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///   </returns>
        public ICollection<HttpLinkRelation> Keys => this.linkDictionary.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        ///   </returns>
        public ICollection<IEnumerable<HttpLink>> Values => this.linkDictionary.Values;


        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public void Add(KeyValuePair<HttpLinkRelation, IEnumerable<HttpLink>> item)
        {
            this.Add(item.Key, item.Value);
        }


        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="relation">The relation of the <paramref name="links"/>.</param>
        /// <param name="links">The list of <see cref="HttpLink"/> to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public void Add(HttpLinkRelation relation, IEnumerable<HttpLink> links)
        {
            foreach (var link in links)
            {
                this.Add(relation, link);
            }
        }


        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public void Clear()
        {
            this.linkDictionary.Clear();
        }


        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<HttpLinkRelation, IEnumerable<HttpLink>> item)
        {
            return this.linkDictionary.Contains(item);
        }


        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="relation">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation"/> is null.
        ///   </exception>
        public bool ContainsKey(HttpLinkRelation relation)
        {
            return this.linkDictionary.ContainsKey(relation);
        }


        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.
        ///   </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.
        ///   </exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
        /// -or-
        ///   <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        ///   </exception>
        public void CopyTo(KeyValuePair<HttpLinkRelation, IEnumerable<HttpLink>>[] array, int arrayIndex)
        {
            this.linkDictionary.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<HttpLinkRelation, IEnumerable<HttpLink>>> GetEnumerator()
        {
            return this.linkDictionary.GetEnumerator();
        }


        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ///   </exception>
        public bool Remove(KeyValuePair<HttpLinkRelation, IEnumerable<HttpLink>> item)
        {
            return this.linkDictionary.Remove(item);
        }


        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="relation">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="relation"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation"/> is null.
        ///   </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        ///   </exception>
        public bool Remove(HttpLinkRelation relation)
        {
            return this.linkDictionary.Remove(relation);
        }


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="relation">The relation of the <paramref name="links"/> to get.</param>
        /// <param name="links">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="links"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation"/> is null.
        ///   </exception>
        public bool TryGetValue(HttpLinkRelation relation, out IEnumerable<HttpLink> links)
        {
            return this.linkDictionary.TryGetValue(relation, out links);
        }


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="link">The <see cref="HttpLink" /> to add.</param>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public void Add(HttpLink link)
        {
            this.Add(link.Relation, link);
        }


        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="relation">The relation of the <paramref name="link"/>.</param>
        /// <param name="link">The <see cref="HttpLink" /> to add.</param>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public void Add(HttpLinkRelation relation, HttpLink link)
        {
            if (this.ContainsKey(relation))
            {
                var list = (List<HttpLink>)this[relation];
                var index = list.IndexOf(link);

                if (index != -1)
                {
                    list[index] = link;
                }
                else
                {
                    list.Add(link);
                }
            }
            else
            {
                this.linkDictionary.Add(relation, new List<HttpLink>(new[] { link }));
            }
        }


        /// <summary>
        /// Parses the specified RFC 5899 Link Header into a new instance of <see cref="HttpLinkHeader"/>.
        /// </summary>
        /// <param name="linkHeader">The RFC 5899 HTTP Link Header.</param>
        /// <returns>
        /// A new instance of <see cref="HttpLinkHeader"/>.
        /// </returns>
        public static HttpLinkHeader Parse(string linkHeader)
        {
            if (string.IsNullOrEmpty(linkHeader))
            {
                throw new ArgumentNullException(nameof(linkHeader));
            }

            var links = linkHeader
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(HttpLink.Parse)
                .ToArray();

            return new HttpLinkHeader(links);
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Join(", ", this.Values.SelectMany(links => links.Select(link => link.ToString())));
        }
    }
}
