namespace Nancy.Tests.Unit.Json
{
    using System;

    public class TestConverterType : IEquatable<TestConverterType>
	{
		public int Data;

		public bool Equals(TestConverterType other)
		{
			if (other == null)
				return false;

			return (this.Data == other.Data);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TestConverterType);
		}

		public override int GetHashCode()
		{
			return this.Data.GetHashCode();
		}
	}
}
