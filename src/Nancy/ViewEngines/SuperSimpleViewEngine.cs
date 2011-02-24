namespace Nancy.ViewEngines
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A super-simple view engine
    /// </summary>
    public class SuperSimpleViewEngine
    {
        /// <summary>
        /// Compiled Regex for single substitutions
        /// </summary>
        private Regex singleSubstitutionsRegEx = new Regex(@"@Model\.(?<ParameterName>[a-zA-Z0-9-_]*)", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for each blocks
        /// </summary>
        private Regex eachSubstitutionRegEx = new Regex(@"@Each\.(?<ParameterName>[a-zA-Z0-9-_]*)(?<Contents>.*?)@EndEach", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled Regex for each block current substitutions
        /// </summary>
        private Regex eachItemSubstitutionRegEx = new Regex("@Current", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for if blocks
        /// </summary>
        private Regex ifSubstitutionRegEx = new Regex(@"@If\.(?<ParameterName>[a-zA-Z0-9-_]*)(?<Contents>.*?)@EndIf", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled Regex for ifnot blocks
        /// </summary>
        private Regex ifNotSubstitutionRegEx = new Regex(@"@IfNot\.(?<ParameterName>[a-zA-Z0-9-_]*)(?<Contents>.*?)@EndIf", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// View engine transform processors
        /// </summary>
        private List<Func<string, object, Func<object, string, object>, string>> processors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngine"/> class.
        /// </summary>
        public SuperSimpleViewEngine()
        {
            this.processors = new List<Func<string, object, Func<object, string, object>, string>>()
                {
                    this.PerformSingleSubstitutions,
                    this.PerformEachSubstitutions,
                    this.PerformConditionalSubstitutions,
                };
        }

        /// <summary>
        /// Renders a template
        /// </summary>
        /// <param name="template">
        /// The template to render.
        /// </param>
        /// <param name="model">
        /// The model to user for rendering.
        /// </param>
        /// <returns>
        /// A string containing the expanded template.
        /// </returns>
        public string Render(string template, dynamic model)
        {
            var propertyExtractor = this.GetPropertyExtractor(model);

            return this.processors.Aggregate(template, (current, processor) => processor(current, model, propertyExtractor));
        }

        /// <summary>
        /// <para>
        /// Gets the correct property extractor for the given model.
        /// </para>
        /// <para>
        /// Anonymous types, standard types and ExpandoObject are supported.
        /// Arbitrary dynamics (implementing IDynamicMetaObjectProvicer) are not, unless
        /// they also implmennt IDictionary string, object for accessing properties.
        /// </para>
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// Delegate for getting properties - delegate returns a value or null if not there.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Model type is not supported.
        /// </exception>
        private Func<object, string, object> GetPropertyExtractor(object model)
        {
            if (!typeof(IDynamicMetaObjectProvider).IsAssignableFrom(model.GetType()))
            {
                return this.GetStandardTypePropertyExtractor(model);
            }

            if (typeof(IDictionary<string, object>).IsAssignableFrom(model.GetType()))
            {
                return this.DynamicDictionaryPropertyExtractor;
            }

            throw new ArgumentException("model must be a standard type or implement IDictionary<string, object>", "model");
        }

        /// <summary>
        /// <para>
        /// Returns the standard property extractor.
        /// </para>
        /// <para>
        /// Model properties are enumerated once and a closure is returned that captures them.
        /// </para>
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>Delegate for getting properties - delegate returns a value or null if not there.</returns>
        private Func<object, string, object> GetStandardTypePropertyExtractor(object model)
        {
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            return (mdl, propName) =>
            {
                var property =
                    properties.Where(p => String.Equals(p.Name, propName, StringComparison.InvariantCulture)).
                        FirstOrDefault();

                return property == null ? null : property.GetValue(mdl, null);
            };
        }

        /// <summary>
        /// A property extractor designed for ExpandoObject, but also for any
        /// type that implements IDictionary string object for accessing its
        /// properties.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// Object if property is found, null if not.
        /// </returns>
        private object DynamicDictionaryPropertyExtractor(object model, string propertyName)
        {
            var dictionaryModel = (IDictionary<string, object>)model;

            object output;
            dictionaryModel.TryGetValue(propertyName, out output);

            return output;
        }

        /// <summary>
        /// Performs single @Model.PropertyName substitutions.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="propertyExtractor">
        /// The property extractor.
        /// </param>
        /// <returns>
        /// Template with @Model.PropertyName blocks expanded
        /// </returns>
        private string PerformSingleSubstitutions(string template, object model, Func<object, string, object> propertyExtractor)
        {
            return this.singleSubstitutionsRegEx.Replace(
                template,
                (m) =>
                {
                    var substitution = propertyExtractor(model, m.Groups["ParameterName"].Value);

                    return substitution == null ? "[ERR!]" : substitution.ToString();
                });
        }

        /// <summary>
        /// Performs @Each.PropertyName substitutions
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="propertyExtractor">
        /// The property extractor.
        /// </param>
        /// <returns>
        /// Template with @Each.PropertyName blocks expanded
        /// </returns>
        private string PerformEachSubstitutions(string template, object model, Func<object, string, object> propertyExtractor)
        {
            return this.eachSubstitutionRegEx.Replace(
                template,
                (m) =>
                {
                    var substitutionObject = propertyExtractor(model, m.Groups["ParameterName"].Value);

                    if (substitutionObject == null)
                    {
                        return "[ERR!]";
                    }

                    var substitutionEnumerable = substitutionObject as IEnumerable;
                    if (substitutionEnumerable == null)
                    {
                        return "[ERR!]";
                    }

                    var result = string.Empty;
                    foreach (var item in substitutionEnumerable)
                    {
                        result += eachItemSubstitutionRegEx.Replace(m.Groups["Contents"].Value, item.ToString());
                    }

                    return result;
                });
        }

        /// <summary>
        /// Performs @If.PropertyName and @IfNot.PropertyName substitutions
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="propertyExtractor">
        /// The property extractor.
        /// </param>
        /// <returns>
        /// Template with @If.PropertyName @IfNot.PropertyName blocks removed/expanded
        /// </returns>
        private string PerformConditionalSubstitutions(string template, object model, Func<object, string, object> propertyExtractor)
        {
            var result = template;

            result = this.ifSubstitutionRegEx.Replace(
                result,
                (m) =>
                {
                    var predicateResult = false;
                    var substitutionObject = propertyExtractor(model, m.Groups["ParameterName"].Value);

                    if (substitutionObject != null)
                    {
                        var substitutionBool = substitutionObject as bool?;
                        if (substitutionBool != null)
                        {
                            predicateResult = substitutionBool.Value;
                        }
                    }

                    return !predicateResult ? String.Empty : m.Groups["Contents"].Value;
                });

            result = this.ifNotSubstitutionRegEx.Replace(
                result,
                (m) =>
                {
                    var predicateResult = true;
                    var substitutionObject = propertyExtractor(model, m.Groups["ParameterName"].Value);

                    if (substitutionObject != null)
                    {
                        var substitutionBool = substitutionObject as bool?;
                        if (substitutionBool != null)
                        {
                            predicateResult = !substitutionBool.Value;
                        }
                    }

                    return !predicateResult ? String.Empty : m.Groups["Contents"].Value;
                });

            return result;
        }
    }
}