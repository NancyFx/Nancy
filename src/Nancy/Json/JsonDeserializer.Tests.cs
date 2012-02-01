#if DEBUG
namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    internal partial class JsonDeserializer
    {
        public class JsonDeserializerTests
        {
            [Fact]
            public void Desirialize_NullTrailingNewLine_False()
            {
                var jsonDeserializer = new JsonDeserializer(new JavaScriptSerializer());
                var input = @"{
    ""Suspended"": false
}";
                var deserialization = jsonDeserializer.Deserialize(input);

                Assert.Equal(((bool)((Dictionary<string, object>)deserialization)["Suspended"]), false);
            }

            [Fact]
            public void Desirialize_NullTrailingNewLine_True()
            {
                var jsonDeserializer = new JsonDeserializer(new JavaScriptSerializer());
                var input = @"{
    ""Suspended"": true
}";
                var deserialization = jsonDeserializer.Deserialize(input);

                Assert.Equal(((bool)((Dictionary<string, object>)deserialization)["Suspended"]), true);
            }

            [Fact]
            public void Desirialize_NullTrailingNewLine_Null()
            {
                var jsonDeserializer = new JsonDeserializer(new JavaScriptSerializer());
                var input = @"{
    ""Suspended"": null
}";
                var deserialization = jsonDeserializer.Deserialize(input);

                Assert.Equal((((Dictionary<string, object>)deserialization)["Suspended"]), null);
            }

            [Fact]
            public void Desirialize_NullTrailingNewLine_int()
            {
                var jsonDeserializer = new JsonDeserializer(new JavaScriptSerializer());
                var input = @"{
    ""Suspended"": 2
}";
                var deserialization = jsonDeserializer.Deserialize(input);

                Assert.Equal(((int)((Dictionary<string, object>)deserialization)["Suspended"]), 2);
            }
        }
    }
}
#endif