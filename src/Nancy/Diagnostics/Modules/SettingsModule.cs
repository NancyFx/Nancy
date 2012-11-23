namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBinding;

    public class SettingsModule : DiagnosticModule
    {
        public SettingsModule()
            : base("/settings")
        {
            Get["/"] = _ => {

                var properties = typeof(StaticConfiguration)
                    .GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.PropertyType == typeof(bool));

                var model = from property in properties
                        orderby property.Name
                        let value = (bool) property.GetValue(null, null)
                        let description = GetDescription(property)
                        where !string.IsNullOrEmpty(description)
                        select new {
                            Name = property.Name,
                            Description = description,
                            DisplayName = Regex.Replace(property.Name, "[A-Z]", " $0"),
                            Value = value,
                            Checked = (value) ? "checked='checked'" : string.Empty
                        };
                
                return View["Settings", model];
            };

            Post["/"] = parameters => {

                var model = 
                    this.Bind<SettingsModel>();

                var property = typeof(StaticConfiguration)
                    .GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .SingleOrDefault(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    property.SetValue(null, model.Value, null);
                }
                
                return HttpStatusCode.OK;
            };
        }

        private static string GetDescription(PropertyInfo property)
        {
            var attributes = property
                .GetCustomAttributes(typeof (DescriptionAttribute), false)
                .Cast<DescriptionAttribute>();

            return (!attributes.Any()) ? string.Empty : attributes.First().Description;
        }
    }

    public class SettingsModel
    {
        public string Name { get; set; }

        public bool Value { get; set; }
    }
}