namespace Nancy.Tests.Functional.Modules
{
    using System;
    using System.Globalization;
    using Nancy.Extensions;
    using Nancy.IO;
    using Nancy.ModelBinding;

    public class SerializerTestModule : NancyModule
    {
        public SerializerTestModule()
        {
            Get("/serializer/{date}", args =>
            {
                var stringparamDate = (string)args.date;
                var dateParsed = DateTime.ParseExact(stringparamDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                return new FakeSerializerModel() { CreatedOn = dateParsed };
            });

            Post("/serializer", args =>
            {
                var model = new FakeSerializerModel { CreatedOn = new DateTime(2014, 01, 30, 0, 0, 0, DateTimeKind.Utc) };
                this.BindTo(model);
                return model;
            });

            Post("/serializer/date", args =>
            {
                var s = ((RequestStream)this.Request.Body).AsString();
                var model = this.Bind<FakeSerializerModel>();
                return model;
            });
        }
    }

    public class FakeSerializerModel
    {
        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }
    }
}
