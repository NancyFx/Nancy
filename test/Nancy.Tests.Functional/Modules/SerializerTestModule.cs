namespace Nancy.Tests.Functional.Modules
{
    using System;
    using System.Globalization;
    using Nancy.ModelBinding;

    public class SerializerTestModule : LegacyNancyModule
    {
        public SerializerTestModule()
        {
            Get["/serializer/{date}"] = parameters =>
            {
                var stringparamDate = (string)parameters.date;
                var dateParsed = DateTime.ParseExact(stringparamDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                return new FakeSerializerModel() { CreatedOn = dateParsed };
            };

            Post["/serializer"] = _ =>
            {
                var model = new FakeSerializerModel { CreatedOn = new DateTime(2014, 01, 30) };
                this.BindTo(model);
                return model;
            };
        }
    }

    public class FakeSerializerModel
    {
        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }
    }
}
