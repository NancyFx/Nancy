namespace Nancy.Tests.Unit.Json
{
    using System;
    using System.Collections.Generic;

    using Nancy.Json;

    public class TestPrimitiveConverter : JavaScriptPrimitiveConverter
	{
		public override IEnumerable<Type> SupportedTypes
		{
			get { yield return typeof(TestPrimitiveConverterType); }
		}

		public override object Deserialize(object primitiveValue, Type type, JavaScriptSerializer serializer)
		{
			if (type != typeof(TestPrimitiveConverterType))
				return null;

			var data = new TestPrimitiveConverterType();

			data.Data = Convert.ToInt32(primitiveValue);

			return data;
		}

		public override object Serialize(object obj, JavaScriptSerializer serializer)
		{
			var data = obj as TestPrimitiveConverterType;

			if (data == null)
				return null;
			else
				return data.Data;
		}
	}
}
