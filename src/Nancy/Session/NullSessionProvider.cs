namespace Nancy.Session
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class NullSessionProvider : ISession
    {
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new InvalidOperationException("Session support is not enabled.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The number of session values
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                throw new InvalidOperationException("Session support is not enabled.");
            }
        }

        /// <summary>
        /// Deletes the session and all associated information
        /// </summary>
        public void DeleteAll()
        {
            throw new InvalidOperationException("Session support is not enabled.");
        }

        /// <summary>
        /// Deletes the specific key from the session
        /// </summary>
        public void Delete(string key)
        {
            throw new InvalidOperationException("Session support is not enabled.");
        }

        /// <summary>
        /// Retrieves the value from the session
        /// </summary>        
        public object this[string key]
        {
            get
            {
                throw new InvalidOperationException("Session support is not enabled.");
            }

            set
            {
                throw new InvalidOperationException("Session support is not enabled.");
            }
        }

        public bool HasChanged
        {
            get
            {
                return false;
            }
        }
    }
}