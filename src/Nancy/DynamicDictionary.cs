namespace Nancy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    
    public class DynamicDictionary : DynamicObject, IEquatable<DynamicDictionary>, IHideObjectMembers, IEnumerable<string>
    {
        private readonly Dictionary<string, object> dictionary =
            new Dictionary<string, object>(StaticConfiguration.CaseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Returns an empty dynamic dictionary.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/> instance.</value>
        public static DynamicDictionary Empty
        {
            get
            {
                return new DynamicDictionary();
            }
        }

        /// <summary>
        /// Creates a dynamic dictionary from an <see cref="IDictionary{TKey,TValue}"/> instance.
        /// </summary>
        /// <param name="values">An <see cref="IDictionary{TKey,TValue}"/> instance, that the dynamic dictionary should be created from.</param>
        /// <returns>An <see cref="DynamicDictionary"/> instance.</returns>
        public static DynamicDictionary Create(IDictionary<string, object> values)
        {
            var instance = new DynamicDictionary();

            foreach (var key in values.Keys)
            {
                instance[key] = values[key];
            }

            return instance;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
        	this[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!dictionary.TryGetValue(binder.Name, out result))
            {
                result = new DynamicDictionaryValue(null);
            }

            return true;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> that contains dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> that contains dynamic member names.</returns>
        public IEnumerator<string> GetEnumerator() {
            return dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>A <see cref="IEnumerator"/> that contains dynamic member names.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the <see cref="DynamicDictionaryValue"/> with the specified name.
        /// </summary>
        /// <value>A <see cref="DynamicDictionaryValue"/> instance containing a value.</value>
        public dynamic this[string name]
        {
            get
            {
                name = GetNeutralKey(name);

                dynamic member;
                if (!dictionary.TryGetValue(name, out member))
                {
                    member = new DynamicDictionaryValue(null);
                }

                return member;
            }
            set
            {
                name = GetNeutralKey(name);

                dictionary[name] = value is DynamicDictionaryValue ? value : new DynamicDictionaryValue(value);
            }
        }

        /// <summary>
        /// Indicates whether the current <see cref="DynamicDictionary"/> is equal to another object of the same type.
        /// </summary>
        /// <returns><see langword="true"/> if the current instance is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        /// <param name="other">An <see cref="DynamicDictionary"/> instance to compare with this instance.</param>
        public bool Equals(DynamicDictionary other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other.dictionary, this.dictionary);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof (DynamicDictionary) && this.Equals((DynamicDictionary) obj);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="DynamicDictionary"/>.
        /// </summary>
        /// <returns> A hash code for this <see cref="DynamicDictionary"/>, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return (dictionary != null ? dictionary.GetHashCode() : 0);
        }

        private static string GetNeutralKey(string key)
        {
            return key.Replace("-", string.Empty);
        }
    }
}