using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Xunit;

namespace Nancy.Tests.Unit
{
    public class DynamicDictionaryFixture
    {
        private dynamic _Dictionary;

        /// <summary>
        /// Initializes a new instance of the DynamicDictionaryFixture class.
        /// </summary>
        public DynamicDictionaryFixture()
        {
            _Dictionary = new DynamicDictionary();
            _Dictionary["TestString"] = "Testing";
            _Dictionary["TestInt"] = 2;
        }

        [Fact]
        public void Should_Return_Actual_String_Value_When_ToString_Called_On_String_Entry()
        {
            string result = _Dictionary.TestString.ToString();

            result.ShouldEqual("Testing");
        }

        [Fact]
        public void Should_Return_String_Representation_Of_Value_When_ToString_Called_On_Int_Entry()
        {
            string result = _Dictionary.TestInt.ToString();

            result.ShouldEqual("2");
        }
    }
}
