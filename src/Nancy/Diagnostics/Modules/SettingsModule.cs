namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Nancy.ModelBinding;

    /// <summary>
    /// Nancy module for diagnostic settings.
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.DiagnosticModule" />
    public class SettingsModule : DiagnosticModule
    {
        private static readonly IEnumerable<Type> Types = new[] { typeof(StaticConfiguration) }.Union(
                                                                  typeof(StaticConfiguration).GetNestedTypes(BindingFlags.Static | BindingFlags.Public));

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsModule"/> class.
        /// </summary>
        public SettingsModule()
            : base("/settings")
        {
            Get("/", _ =>
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
            });

            Post("/", _ => {

                var model =
                    this.Bind<SettingsModel>();

                var property = GetProperty(model);

                if (property != null)
                {
                    property.SetValue(null, model.Value, null);
                }

                return HttpStatusCode.OK;
            });
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

    /// <summary>
    /// 
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Gets or sets the name for the setting.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value for this setting.
        /// </summary>
        /// <value>
        ///   <c>true</c> or <c>false</c>.
        /// </value>
        public bool Value { get; set; }
    }
}