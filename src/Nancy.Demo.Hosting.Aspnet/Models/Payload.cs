namespace Nancy.Demo.Hosting.Aspnet.Models
{
    using System;

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

            return obj.GetType() == typeof(Payload) && this.Equals((Payload)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = this.IntValue;
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

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", this.StringValue, this.IntValue, this.BoolValue);
        }
    }
}