namespace Nancy.Tests.Unit.Json
{
    using System;
    using System.Collections.Generic;

    using Nancy.Json;

    public class TestConverter : JavaScriptConverter
	{
		public override IEnumerable<Type> SupportedTypes
		{
			get { yield return typeof(TestConverterType); }
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if (type != typeof(TestConverterType))
				return null;

			var data = new TestConverterType();

			data.Data = Convert.ToInt32(dictionary["DataValue"]);

			return data;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			TestConverterType data = obj as TestConverterType;

			if (data == null)
				return null;

			var fields = new Dictionary<string, object>();

			fields["DataValue"] = data.Data;

			return fields;
		}
	}
}
