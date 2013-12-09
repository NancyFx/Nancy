namespace Nancy.Tests.Functional.Modules
{
    using System;
    using System.Globalization;

    public class SerializerTestModule : NancyModule
    {
        public SerializerTestModule()
        {
            Get["/serializer/{date}"] = parameters =>
            {
                var stringparamDate = (string)parameters.date;
                var dateParsed = DateTime.ParseExact(stringparamDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                return new FakeSerializerModel() { CreatedOn = dateParsed };
            };
        }
    }

    public class FakeSerializerModel
    {
        public DateTime CreatedOn { get; set; }
    }
}
