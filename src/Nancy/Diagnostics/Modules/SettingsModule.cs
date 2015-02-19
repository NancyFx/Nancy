namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Nancy.ModelBinding;

    public class SettingsModule : DiagnosticModule
    {
        private static readonly IEnumerable<Type> Types = new[] { typeof(StaticConfiguration) }.Union(
                                                                  typeof(StaticConfiguration).GetNestedTypes(BindingFlags.Static | BindingFlags.Public));

        public SettingsModule()
            : base("/settings")
        {
            Get["/"] = _ =>
            {
                var properties = Types.SelectMany(t => t.GetProperties(BindingFlags.Static | BindingFlags.Public))
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

                var property = GetProperty(model);

                if (property != null)
                {
                    property.SetValue(null, model.Value, null);
                }

                return HttpStatusCode.OK;
            };
        }

        private static PropertyInfo GetProperty(SettingsModel model)
        {
            return Types.SelectMany(t => t.GetProperties(BindingFlags.Static | BindingFlags.Public))
                        .SingleOrDefault(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetDescription(PropertyInfo property)
        {
            var attributes = property
                .GetCustomAttributes(typeof (DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .ToArray();

            return (!attributes.Any()) ? string.Empty : attributes.First().Description;
        }
    }

    public class SettingsModel
    {
        public string Name { get; set; }

        public bool Value { get; set; }
    }
}