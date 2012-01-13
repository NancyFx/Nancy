namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using ModelBinding;

    public class SettingsModule : DiagnosticModule
    {
        public SettingsModule()
            : base("/settings")
        {
            Get["/"] = _ => {

                var properties = typeof(StaticConfiguration)
                    .GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.PropertyType.Equals(typeof(bool)));

                var model = from property in properties
                        orderby property.Name
                        let value = (bool) property.GetValue(null, null)
                        select new {
                            Name = property.Name,
                            Value = value,
                            Checked = (value) ? "checked='checked'" : string.Empty
                        };

                //var model = properties
                //    .OrderBy(x => x.Name)
                //    .Select(x => new {Name = x.Name, Value = x.GetValue(x, null)});

                return View["Settings", model];
            };

            Post["/"] = parameters =>{

                var model = 
                    this.Bind<SettingsModel>();

                var property = typeof(StaticConfiguration)
                    .GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase))
                    .SingleOrDefault();

                property.SetValue(null, model.Value, null);

                return HttpStatusCode.OK;
            };
        }
    }

    public class SettingsModel
    {
        public string Name { get; set; }

        public bool Value { get; set; }
    }
}