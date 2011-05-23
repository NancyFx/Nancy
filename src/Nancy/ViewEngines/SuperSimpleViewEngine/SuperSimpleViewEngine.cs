namespace Nancy.ViewEngines.SuperSimpleViewEngine
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
        private readonly Regex singleSubstitutionsRegEx = new Regex(@"@(?<Encode>!)?Model(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))+;?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for each blocks
        /// </summary>
        private readonly Regex eachSubstitutionRegEx = new Regex(@"@Each(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?(?<Contents>.*?)@EndEach;?", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled Regex for each block current substitutions
        /// </summary>
        private readonly Regex eachItemSubstitutionRegEx = new Regex(@"@(?<Encode>!)?Current(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for if blocks
        /// </summary>
        private readonly Regex conditionalSubstitutionRegEx = new Regex(@"@If(?<Not>Not)?(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))+;?(?<Contents>.*?)@EndIf;?", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled regex for partial blocks
        /// </summary>
        private readonly Regex partialSubstitutionRegEx = new Regex(@"@Partial\['(?<ViewName>.+)'(?<Model>.[ ]?Model(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*)?\];?", RegexOptions.Compiled);

        /// <summary>
        /// View engine transform processors
        /// </summary>
        private readonly List<Func<string, object, string>> processors;

        /// <summary>
        /// Stores the view engine context
        /// </summary>
        private IViewEngineHost viewEngineHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngine"/> class.
        /// </summary>
        public SuperSimpleViewEngine(IViewEngineHost viewEngineHost)
        {
            this.viewEngineHost = viewEngineHost;

            this.processors = new List<Func<string, object, string>>
                {
                    this.PerformSingleSubstitutions,
                    this.PerformEachSubstitutions,
                    this.PerformConditionalSubstitutions,
                    this.PerformPartialSubstitutions,
                };
        }

        /// <summary>
        /// Renders a template
        /// </summary>
        /// <param name="template">The template to render.</param>
        /// <param name="model">The model to user for rendering.</param>
        /// <returns>A string containing the expanded template.</returns>
        public string Render(string template, dynamic model)
        {
            if (model == null)
            {
                return template;
            }

            return this.processors.Aggregate(template, (current, processor) => processor(current, model));
        }

        /// <summary>
        /// <para>
        /// Gets a property value from the given model.
        /// </para>
        /// <para>
        /// Anonymous types, standard types and ExpandoObject are supported.
        /// Arbitrary dynamics (implementing IDynamicMetaObjectProvicer) are not, unless
        /// they also implmennt IDictionary string, object for accessing properties.
        /// </para>
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name to evaluate.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was sucessful, Item2 being the value.</returns>
        /// <exception cref="ArgumentException">Model type is not supported.</exception>
        private static Tuple<bool, object> GetPropertyValue(object model, string propertyName)
        {
            if (model == null || String.IsNullOrEmpty(propertyName))
            {
                return new Tuple<bool, object>(false, null);
            }

            if (!typeof(IDynamicMetaObjectProvider).IsAssignableFrom(model.GetType()))
            {
                return StandardTypePropertyEvaluator(model, propertyName);
            }

            if (typeof(IDictionary<string, object>).IsAssignableFrom(model.GetType()))
            {
                return DynamicDictionaryPropertyEvaluator(model, propertyName);
            }

            throw new ArgumentException("model must be a standard type or implement IDictionary<string, object>", "model");
        }

        /// <summary>
        /// A property extractor for standard types.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was sucessful, Item2 being the value.</returns>
        private static Tuple<bool, object> StandardTypePropertyEvaluator(object model, string propertyName)
        {
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var property =
                properties.Where(p => String.Equals(p.Name, propertyName, StringComparison.InvariantCulture)).
                FirstOrDefault();

            return property == null ? new Tuple<bool, object>(false, null) : new Tuple<bool, object>(true, property.GetValue(model, null));
        }

        /// <summary>
        /// A property extractor designed for ExpandoObject, but also for any
        /// type that implements IDictionary string object for accessing its
        /// properties.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was sucessful, Item2 being the value.</returns>
        private static Tuple<bool, object> DynamicDictionaryPropertyEvaluator(object model, string propertyName)
        {
            var dictionaryModel = (IDictionary<string, object>)model;

            object output;
            return !dictionaryModel.TryGetValue(propertyName, out output) ? new Tuple<bool, object>(false, null) : new Tuple<bool, object>(true, output);
        }

        /// <summary>
        /// Gets an IEnumerable of capture group values
        /// </summary>
        /// <param name="m">The match to use.</param>
        /// <param name="groupName">Group name containing the capture group.</param>
        /// <returns>IEnumerable of capture group values as strings.</returns>
        private static IEnumerable<string> GetCaptureGroupValues(Match m, string groupName)
        {
            return m.Groups[groupName].Captures.Cast<Capture>().Select(c => c.Value);
        }

        /// <summary>
        /// Gets a property value from a collection of nested parameter names
        /// </summary>
        /// <param name="model">The model containing properties.</param>
        /// <param name="parameters">A collection of nested parameters (e.g. User, Name</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was sucessful, Item2 being the value.</returns>
        private static Tuple<bool, object> GetPropertyValueFromParameterCollection(object model, IEnumerable<string> parameters)
        {
            if (parameters == null)
            {
                return new Tuple<bool, object>(true, model);
            }

            var currentObject = model;
            Tuple<bool, object> currentResult;

            foreach (var parameter in parameters)
            {
                currentResult = GetPropertyValue(currentObject, parameter);

                if (currentResult.Item1 == false)
                {
                    return new Tuple<bool, object>(false, null);
                }

                currentObject = currentResult.Item2;
            }

            return new Tuple<bool, object>(true, currentObject);
        }

        /// <summary>
        /// Gets the predicate result for an If or IfNot block
        /// </summary>
        /// <param name="item">The item to evaluate</param>
        /// <param name="properties">Property list to evaluate</param>
        /// <returns>Bool representing the predicate result</returns>
        private static bool GetPredicateResult(object item, IEnumerable<string> properties)
        {
            var substitutionObject = GetPropertyValueFromParameterCollection(item, properties);

            if (substitutionObject.Item1 == false && properties.Last().StartsWith("Has"))
            {
                var newProperties =
                    properties.Take(properties.Count() - 1).Concat(new[] { properties.Last().Substring(3) });

                substitutionObject = GetPropertyValueFromParameterCollection(item, newProperties);

                return GetHasPredicateResultFromSubstitutionObject(substitutionObject.Item2);
            }

            if (substitutionObject.Item2 == null)
            {
                return false;
            }

            return GetPredicateResultFromSubstitutionObject(substitutionObject.Item2);
        }

        /// <summary>
        /// Returns the predicate result if the substitionObject is a valid bool
        /// </summary>
        /// <param name="substitutionObject">The substitution object.</param>
        /// <returns>Bool value of the substitutionObject, or false if unable to cast.</returns>
        private static bool GetPredicateResultFromSubstitutionObject(object substitutionObject)
        {
            var predicateResult = false;

            var substitutionBool = substitutionObject as bool?;
            if (substitutionBool != null)
            {
                predicateResult = substitutionBool.Value;
            }

            return predicateResult;
        }

        /// <summary>
        /// Returns the predicate result if the substitionObject is a valid ICollection
        /// </summary>
        /// <param name="substitutionObject">The substitution object.</param>
        /// <returns>Bool value of the whether the ICollection has items, or false if unable to cast.</returns>
        private static bool GetHasPredicateResultFromSubstitutionObject(object substitutionObject)
        {
            var predicateResult = false;

            var substitutionCollection = substitutionObject as ICollection;
            if (substitutionCollection != null)
            {
                predicateResult = substitutionCollection.Count != 0;
            }

            return predicateResult;
        }

        /// <summary>
        /// Performs single @Model.PropertyName substitutions.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <returns>Template with @Model.PropertyName blocks expanded.</returns>
        private string PerformSingleSubstitutions(string template, object model)
        {
            return this.singleSubstitutionsRegEx.Replace(
                template,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var substitution = GetPropertyValueFromParameterCollection(model, properties);

                    if (!substitution.Item1)
                    {
                        return "[ERR!]";
                    }

                    if (substitution.Item2 == null)
                    {
                        return String.Empty;
                    }

                    return m.Groups["Encode"].Success ? this.viewEngineHost.HtmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
                });
        }

        /// <summary>
        /// Performs @Each.PropertyName substitutions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <returns>Template with @Each.PropertyName blocks expanded.</returns>
        private string PerformEachSubstitutions(string template, object model)
        {
            return this.eachSubstitutionRegEx.Replace(
                template,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var substitutionObject = GetPropertyValueFromParameterCollection(model, properties);

                    if (substitutionObject.Item1 == false)
                    {
                        return "[ERR!]";
                    }

                    if (substitutionObject.Item2 == null)
                    {
                        return String.Empty;
                    }

                    var substitutionEnumerable = substitutionObject.Item2 as IEnumerable;
                    if (substitutionEnumerable == null)
                    {
                        return "[ERR!]";
                    }

                    var contents = m.Groups["Contents"].Value;
                    var result = string.Empty;
                    foreach (var item in substitutionEnumerable)
                    {
                        result += ReplaceCurrentMatch(contents, item);
                    }

                    return result;
                });
        }

        /// <summary>
        /// Expand a @Current match inside an @Each iterator
        /// </summary>
        /// <param name="contents">Contents of the @Each block</param>
        /// <param name="item">Current item from the @Each enumerable</param>
        /// <returns>String result of the expansion of the @Each.</returns>
        private string ReplaceCurrentMatch(string contents, object item)
        {
            return this.eachItemSubstitutionRegEx.Replace(
                contents,
                eachMatch =>
                {
                    if (String.IsNullOrEmpty(eachMatch.Groups["ParameterName"].Value))
                    {
                        return eachMatch.Groups["Encode"].Success ? this.viewEngineHost.HtmlEncode(item.ToString()) : item.ToString();
                    }

                    var properties = GetCaptureGroupValues(eachMatch, "ParameterName");

                    var substitution = GetPropertyValueFromParameterCollection(item, properties);

                    if (!substitution.Item1)
                    {
                        return "[ERR!]";
                    }

                    if (substitution.Item2 == null)
                    {
                        return string.Empty;
                    }

                    return eachMatch.Groups["Encode"].Success ? this.viewEngineHost.HtmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
                });
        }

        /// <summary>
        /// Performs @If.PropertyName and @IfNot.PropertyName substitutions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <returns>Template with @If.PropertyName @IfNot.PropertyName blocks removed/expanded.</returns>
        private string PerformConditionalSubstitutions(string template, object model)
        {
            var result = template;

            result = this.conditionalSubstitutionRegEx.Replace(
                result,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var predicateResult = GetPredicateResult(model, properties);

                    if (m.Groups["Not"].Value == "Not")
                    {
                        predicateResult = !predicateResult;
                    }

                    return predicateResult ? m.Groups["Contents"].Value : String.Empty;
                });

            return result;
        }

        /// <summary>
        /// Perform @Partial partial view expansion
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <returns>Template with partials expanded</returns>
        private string PerformPartialSubstitutions(string template, object model)
        {
            var result = template;

            result = this.partialSubstitutionRegEx.Replace(
                result,
                m =>
                {
                    var partialViewName = m.Groups["ViewName"].Value;
                    var partialModel = model;
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    if (m.Groups["Model"].Length > 0)
                    {
                        var modelValue = GetPropertyValueFromParameterCollection(model, properties);

                        if (modelValue.Item1 != true)
                        {
                            return "[ERR!]";
                        }

                        partialModel = modelValue.Item2;
                    }

                    var partialTemplate = this.viewEngineHost.GetTemplate(partialViewName, partialModel);

                    return this.Render(partialTemplate, partialModel);
                });

            return result;
        }
    }
}