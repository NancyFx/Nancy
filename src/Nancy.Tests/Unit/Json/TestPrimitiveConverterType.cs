namespace Nancy.Tests.Unit.Json
{
    using System;

    public class TestPrimitiveConverterType : IEquatable<TestPrimitiveConverterType>
	{
		public int Data;

		public bool Equals(TestPrimitiveConverterType other)
		{
			if (other == null)
				return false;

			return (this.Data == other.Data);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TestPrimitiveConverterType);
		}

		public override int GetHashCode()
		{
			return this.Data.GetHashCode();
		}
	}
}
