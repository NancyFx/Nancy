namespace Nancy.Tests.Unit.Sessions
{
    using System;

    using Nancy.Session;

    using Xunit;

    public class DefaultSessionObjectFormatterFixture
    {
        private DefaultObjectSerializer serializer;

        public DefaultSessionObjectFormatterFixture()
        {
            this.serializer = new DefaultObjectSerializer();
        }

        [Fact]
        public void Should_serialize_and_deserialize_simple_string()
        {
            var input = @"This is a sample string";
            var serialised = this.serializer.Serialize(input);

            var output = (string)this.serializer.Deserialize(serialised);

            output.ShouldEqual(input);
        }

        [Fact]
        public void Should_serialize_and_deserialize_serializable_object()
        {
            var input = new Payload(27, true, "This is some text");
            var serialised = this.serializer.Serialize(input);

            var output = (Payload)this.serializer.Deserialize(serialised);

            output.ShouldEqual(input);
        }

        [Fact]
        public void Should_return_empty_string_when_serializing_null()
        {
            object input = null;
            
            var output = this.serializer.Serialize(input);

            output.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_return_null_when_deserializing_null()
        {
            string input = null;

            var output = this.serializer.Deserialize(input);

            output.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_deserializing_empty_string()
        {
            var input = String.Empty;

            var output = this.serializer.Deserialize(input);

            output.ShouldBeNull();
        }

        [Serializable]
        public class Payload : IEquatable<Payload>
        {
            public int IntValue { get; private set; }

            public bool BoolValue { get; private set; }

            public string StringValue { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public Payload(int intValue, bool boolValue, string stringValue)
            {
                this.IntValue = intValue;
                this.BoolValue = boolValue;
                this.StringValue = stringValue;
            }

            public bool Equals(Payload other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return other.IntValue == this.IntValue && other.BoolValue.Equals(this.BoolValue) && Equals(other.StringValue, this.StringValue);
            }

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

                if (obj.GetType() != typeof(Payload))
                {
                    return false;
                }

                return Equals((Payload)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = this.IntValue;
                    result = (result * 397) ^ this.BoolValue.GetHashCode();
                    result = (result * 397) ^ (this.StringValue != null ? this.StringValue.GetHashCode() : 0);
                    return result;
                }
            }

            public static bool operator ==(Payload left, Payload right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Payload left, Payload right)
            {
                return !Equals(left, right);
            }
        }
    }
}