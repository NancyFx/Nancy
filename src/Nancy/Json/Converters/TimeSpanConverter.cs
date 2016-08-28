namespace Nancy.Json.Converters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Converts a dictionary with time info into a time span instance or vice versa.
    /// </summary>
    /// <seealso cref="Nancy.Json.JavaScriptConverter" />
    public class TimeSpanConverter : JavaScriptConverter
    {
        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <value>
        /// The supported types.
        /// </value>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[] { typeof(TimeSpan) };
            }
        }

        /// <summary>
        /// Deserializes the specified dictionary into a timespan instance.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new TimeSpan(
                this.GetValue(dictionary, "Days"),
                this.GetValue(dictionary, "Hours"),
                this.GetValue(dictionary, "Minutes"),
                this.GetValue(dictionary, "Seconds"),
                this.GetValue(dictionary, "Milliseconds"));
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var timeSpan = (TimeSpan)obj;

            var result = new Dictionary<string, object>
                             {
                                 { "Days", timeSpan.Days },
                                 { "Hours", timeSpan.Hours },
                                 { "Minutes", timeSpan.Minutes },
                                 { "Seconds", timeSpan.Seconds },
                                 { "Milliseconds", timeSpan.Milliseconds }
                             };

            return result;
        }

        private int GetValue(IDictionary<string, object> dictionary, string key)
        {
            const int DefaultValue = 0;

            object value;
            if (!dictionary.TryGetValue(key, out value))
            {
                return DefaultValue;
            }

            if (value is int)
            {
                return (int)value;
            }

            if (value is long)
            {
                return Convert.ToInt32((long)value);
            }

            var valueString = value as string;
            if (valueString == null)
            {
                return DefaultValue;
            }

            int returnValue;
            return !int.TryParse(valueString, out returnValue) ? DefaultValue : returnValue;
        }
    }
}