using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.ViewEngines.NDjango
{
    class DictionaryWrapper
    {
        private DynamicDictionary dictionary;

        public DictionaryWrapper(DynamicDictionary dict)
        {
            dictionary = dict;
        }

        public object this[string key]
        {
            get
            {
                return Unwrap(dictionary[key]);
            }
            set
            {
                dictionary[key] = value;
            }
        }

        private object Unwrap(object o)
        {
            var val = o as DynamicDictionaryValue;
            if (!Object.ReferenceEquals(val, null))
            {
                if (!val.HasValue)
                    return null;
                return val.Value;
            }
            return o;

        }
    }
}
