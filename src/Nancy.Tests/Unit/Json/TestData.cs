namespace Nancy.Tests.Unit.Json
{
    using System;

    public class TestData : IEquatable<TestData>
	{
		public TestConverterType ConverterData;
		public TestPrimitiveConverterType PrimitiveConverterData;

		public bool Equals(TestData other)
		{
			if (other == null)
				return false;

			if ((this.ConverterData != null) != (other.ConverterData != null))
				return false;

			if ((this.PrimitiveConverterData != null) != (other.PrimitiveConverterData != null))
				return false;

			return
				(ReferenceEquals(this.ConverterData, other.ConverterData) || this.ConverterData.Equals(other.ConverterData)) &&
				(ReferenceEquals(this.PrimitiveConverterData, other.PrimitiveConverterData) || this.PrimitiveConverterData.Equals(other.PrimitiveConverterData));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TestData);
		}

		public override int GetHashCode()
		{
			return ConverterData.GetHashCode() ^ PrimitiveConverterData.GetHashCode();
		}
	}
}
