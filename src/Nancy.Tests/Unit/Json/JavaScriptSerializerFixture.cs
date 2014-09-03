namespace Nancy.Tests.Unit.Json
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FakeItEasy;
	using Nancy.IO;
	using Nancy.Json;
	using Xunit;
	using Xunit.Extensions;
	using Xunit.Sdk;

	public class JavaScriptSerializerFixture
	{
		[Fact]
		public void Should_register_converters_when_asked()
		{
			// Given
			JsonSettings.Converters.Add(new TestConverter());
			JsonSettings.PrimitiveConverters.Add(new TestPrimitiveConverter());

			var defaultSerializer = new JavaScriptSerializer();

			// When
			var serializer = new JavaScriptSerializer(
				registerConverters: true,
				resolver: null,
				maxJsonLength: defaultSerializer.MaxJsonLength,
				recursionLimit: defaultSerializer.RecursionLimit,
				retainCasing: defaultSerializer.RetainCasing,
				iso8601DateFormat: defaultSerializer.ISO8601DateFormat);

			var data =
				new TestData()
				{
					ConverterData =
						new TestConverterType()
						{
							Data = 42,
						},

					PrimitiveConverterData =
						new TestPrimitiveConverterType()
						{
							Data = 1701,
						},
				};

			const string ExpectedJSON = @"{""converterData"":{""dataValue"":42},""primitiveConverterData"":1701}";

			// Then
			serializer.Serialize(data).ShouldEqual(ExpectedJSON);

			serializer.Deserialize<TestData>(ExpectedJSON).ShouldEqual(data);
		}

		[Fact]
		public void Should_not_register_converters_when_not_asked()
		{
			// Given
			JsonSettings.Converters.Add(new TestConverter());
			JsonSettings.PrimitiveConverters.Add(new TestPrimitiveConverter());

			var defaultSerializer = new JavaScriptSerializer();

			// When
			var serializer = new JavaScriptSerializer(
				registerConverters: false,
				resolver: null,
				maxJsonLength: defaultSerializer.MaxJsonLength,
				recursionLimit: defaultSerializer.RecursionLimit,
				retainCasing: defaultSerializer.RetainCasing,
				iso8601DateFormat: defaultSerializer.ISO8601DateFormat);

			var data =
				new TestData()
				{
					ConverterData =
						new TestConverterType()
						{
							Data = 42,
						},

					PrimitiveConverterData =
						new TestPrimitiveConverterType()
						{
							Data = 1701,
						},
				};

			const string ExpectedJSON = @"{""converterData"":{""data"":42},""primitiveConverterData"":{""data"":1701}}";

			// Then
			serializer.Serialize(data).ShouldEqual(ExpectedJSON);

			serializer.Deserialize<TestData>(ExpectedJSON).ShouldEqual(data);
		}

		[Fact]
		public void Should_use_primitive_converter_when_available()
		{
			// When
			var serializer = new JavaScriptSerializer();

			serializer.RegisterConverters(new JavaScriptPrimitiveConverter[] { new TestPrimitiveConverter() });

			// Then
			serializer.Serialize(new TestPrimitiveConverterType() { Data = 12345 }).ShouldEqual("12345");

			serializer.Deserialize<TestPrimitiveConverterType>("12345").ShouldEqual(new TestPrimitiveConverterType() { Data = 12345 });
		}

		[Fact]
		public void Should_not_use_primitive_converter_for_wrong_type()
		{
			// When
			var serializer = new JavaScriptSerializer();

			serializer.RegisterConverters(new JavaScriptPrimitiveConverter[] { new TestPrimitiveConverter() });

			// Then
			serializer.Serialize(new TestConverterType() { Data = 12345 }).ShouldEqual(@"{""data"":12345}");

			serializer.Deserialize<TestConverterType>(@"{""data"":12345}").ShouldEqual(new TestConverterType() { Data = 12345 });

			try
			{
				serializer.Deserialize<TestConverterType>("12345");
				throw new ThrowsException(typeof(InvalidCastException));
			}
			catch { }
		}
	}
}
