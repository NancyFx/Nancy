namespace Nancy.Tests.Unit.Extensions
{
    using System;

    using Nancy.Extensions;

    using Xunit;

    public class TypeExtensionsFixture
    {
        [Fact]
        public void Should_test_non_numeric_types()
        {
            Assert.False(TypeExtensions.IsNumeric(null));
            Assert.False(typeof(object).IsNumeric());
            Assert.False(typeof(DBNull).IsNumeric());
            Assert.False(typeof(bool).IsNumeric());
            Assert.False(typeof(char).IsNumeric());
            Assert.False(typeof(DateTime).IsNumeric());
            Assert.False(typeof(string).IsNumeric());
        }

        [Fact]
        public void Should_test_Arrays_of_numeric_and_non_numeric_types()
        {
            Assert.False(typeof(object[]).IsNumeric());
            Assert.False(typeof(DBNull[]).IsNumeric());
            Assert.False(typeof(bool[]).IsNumeric());
            Assert.False(typeof(char[]).IsNumeric());
            Assert.False(typeof(DateTime[]).IsNumeric());
            Assert.False(typeof(string[]).IsNumeric());
            Assert.False(typeof(byte[]).IsNumeric());
            Assert.False(typeof(decimal[]).IsNumeric());
            Assert.False(typeof(double[]).IsNumeric());
            Assert.False(typeof(short[]).IsNumeric());
            Assert.False(typeof(int[]).IsNumeric());
            Assert.False(typeof(long[]).IsNumeric());
            Assert.False(typeof(sbyte[]).IsNumeric());
            Assert.False(typeof(float[]).IsNumeric());
            Assert.False(typeof(ushort[]).IsNumeric());
            Assert.False(typeof(uint[]).IsNumeric());
            Assert.False(typeof(ulong[]).IsNumeric());
        }

        [Fact]
        public void Should_test_numeric_types()
        {
            Assert.True(typeof(byte).IsNumeric());
            Assert.True(typeof(decimal).IsNumeric());
            Assert.True(typeof(double).IsNumeric());
            Assert.True(typeof(short).IsNumeric());
            Assert.True(typeof(int).IsNumeric());
            Assert.True(typeof(long).IsNumeric());
            Assert.True(typeof(sbyte).IsNumeric());
            Assert.True(typeof(float).IsNumeric());
            Assert.True(typeof(ushort).IsNumeric());
            Assert.True(typeof(uint).IsNumeric());
            Assert.True(typeof(ulong).IsNumeric());
        }

        [Fact]
        public void Should_test_nullable_non_numeric_types()
        {
            Assert.False(typeof(bool?).IsNumeric());
            Assert.False(typeof(char?).IsNumeric());
            Assert.False(typeof(DateTime?).IsNumeric());
        }

        [Fact]
        public void Should_test_nullable_numeric_types()
        {
            Assert.True(typeof(byte?).IsNumeric());
            Assert.True(typeof(decimal?).IsNumeric());
            Assert.True(typeof(double?).IsNumeric());
            Assert.True(typeof(short?).IsNumeric());
            Assert.True(typeof(int?).IsNumeric());
            Assert.True(typeof(long?).IsNumeric());
            Assert.True(typeof(sbyte?).IsNumeric());
            Assert.True(typeof(float?).IsNumeric());
            Assert.True(typeof(ushort?).IsNumeric());
            Assert.True(typeof(uint?).IsNumeric());
            Assert.True(typeof(ulong?).IsNumeric());
        }

        [Fact]
        public void Should_test_non_numeric_gettype()
        {
            Assert.False((new object()).GetType().IsNumeric());
            Assert.False(DBNull.Value.GetType().IsNumeric());
            Assert.False(true.GetType().IsNumeric());
            Assert.False('a'.GetType().IsNumeric());
            Assert.False((new DateTime(2009, 1, 1)).GetType().IsNumeric());
            Assert.False(string.Empty.GetType().IsNumeric());
        }

        [Fact]
        public void Should_test_numeric_gettype()
        {
            Assert.True((new byte()).GetType().IsNumeric());
            Assert.True(43.2m.GetType().IsNumeric());
            Assert.True(43.2d.GetType().IsNumeric());
            Assert.True(((short)2).GetType().IsNumeric());
            Assert.True(((int)2).GetType().IsNumeric());
            Assert.True(((long)2).GetType().IsNumeric());
            Assert.True(((sbyte)2).GetType().IsNumeric());
            Assert.True(2f.GetType().IsNumeric());
            Assert.True(((ushort)2).GetType().IsNumeric());
            Assert.True(((uint)2).GetType().IsNumeric());
            Assert.True(((ulong)2).GetType().IsNumeric());
        }

        [Fact]
        public void Should_test_nullable_non_numeric_types_gettype()
        {
            bool? nullableBool = true;
            Assert.False(nullableBool.GetType().IsNumeric());

            char? nullableChar = ' ';
            Assert.False(nullableChar.GetType().IsNumeric());
            
            DateTime? nullableDateTime = new DateTime(2009, 1, 1);
            Assert.False(nullableDateTime.GetType().IsNumeric());
        }

        [Fact]
        public void Should_test_nullable_numeric_types_gettype()
        {
            byte? nullableByte = 12;
            Assert.True(nullableByte.GetType().IsNumeric());

            decimal? nullableDecimal = 12.2m;
            Assert.True(nullableDecimal.GetType().IsNumeric());

            double? nullableDouble = 12.32;
            Assert.True(nullableDouble.GetType().IsNumeric());

            short? nullableInt16 = 12;
            Assert.True(nullableInt16.GetType().IsNumeric());

            short? nullableInt32 = 12;
            Assert.True(nullableInt32.GetType().IsNumeric());

            short? nullableInt64 = 12;
            Assert.True(nullableInt64.GetType().IsNumeric());

            sbyte? nullableSByte = 12;
            Assert.True(nullableSByte.GetType().IsNumeric());

            float? nullableSingle = 3.2f;
            Assert.True(nullableSingle.GetType().IsNumeric());

            ushort? nullableUInt16 = 12;
            Assert.True(nullableUInt16.GetType().IsNumeric());

            ushort? nullableUInt32 = 12;
            Assert.True(nullableUInt32.GetType().IsNumeric());

            ushort? nullableUInt64 = 12;
            Assert.True(nullableUInt64.GetType().IsNumeric());

        }
    }
}
