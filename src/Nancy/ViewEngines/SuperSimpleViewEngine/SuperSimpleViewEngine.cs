namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
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
        private static readonly Regex SingleSubstitutionsRegEx = new Regex(@"@(?<Encode>!)?Model(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for context subsituations
        /// </summary>
        private static readonly Regex ContextSubstitutionsRegEx = new Regex(@"@(?<Encode>!)?Context(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for each blocks
        /// </summary>
        private static readonly Regex EachSubstitutionRegEx = new Regex(@"@Each(?:\.(?<ModelSource>(Model|Context)+))?(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?(?<Contents>.*?)@EndEach;?", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled Regex for each block current substitutions
        /// </summary>
        private static readonly Regex EachItemSubstitutionRegEx = new Regex(@"@(?<Encode>!)?Current(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*;?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled Regex for if blocks
        /// </summary>
        private static readonly Regex ConditionalSubstitutionRegEx = new Regex(@"@If(?<Not>Not)?(?<Null>Null)?(?:\.(?<ModelSource>(Model|Context)+))?(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))+;?(?<Contents>.*?)@EndIf;?", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled regex for partial blocks
        /// </summary>
        private static readonly Regex PartialSubstitutionRegEx = new Regex(@"@Partial\['(?<ViewName>.+)'(?<Model>.[ ]?Model(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*)?\];?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled RegEx for section block declarations
        /// </summary>
        private static readonly Regex SectionDeclarationRegEx = new Regex(@"@Section\[\'(?<SectionName>.+?)\'\];?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled RegEx for section block contents
        /// </summary>
        private static readonly Regex SectionContentsRegEx = new Regex(@"(?:@Section\[\'(?<SectionName>.+?)\'\];?(?<SectionContents>.*?)@EndSection;?)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Compiled RegEx for master page declaration
        /// </summary>
        private static readonly Regex MasterPageHeaderRegEx = new Regex(@"^(?:@Master\[\'(?<MasterPage>.+?)\'\]);?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled RegEx for path expansion
        /// </summary>
        private static readonly Regex PathExpansionRegEx = new Regex(@"(?:@Path\[\'(?<Path>.+?)\'\]);?", RegexOptions.Compiled);

        /// <summary>
        /// Compiled RegEx for the CSRF anti forgery token
        /// </summary>
        private static readonly Regex AntiForgeryTokenRegEx = new Regex(@"@AntiForgeryToken;?", RegexOptions.Compiled);

        /// <summary>
        /// View engine transform processors
        /// </summary>
        private readonly List<Func<string, object, IViewEngineHost, string>> processors;

        /// <summary>
        /// View engine extensions
        /// </summary>
        private readonly IEnumerable<ISuperSimpleViewEngineMatcher> matchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngine"/> class.
        /// </summary>
        public SuperSimpleViewEngine()
            : this(Enumerable.Empty<ISuperSimpleViewEngineMatcher>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngine"/> class, using
        /// the provided <see cref="ISuperSimpleViewEngineMatcher"/> extensions.
        /// </summary>
        /// <param name="matchers">The matchers to use with the engine.</param>
        public SuperSimpleViewEngine(IEnumerable<ISuperSimpleViewEngineMatcher> matchers)
        {
            this.matchers = matchers ?? Enumerable.Empty<ISuperSimpleViewEngineMatcher>();

            this.processors = new List<Func<string, object, IViewEngineHost, string>>
            {
                PerformSingleSubstitutions,
                PerformContextSubstitutions,
                PerformEachSubstitutions,
                PerformConditionalSubstitutions,
                PerformPathSubstitutions,
                PerformAntiForgeryTokenSubstitutions,
                this.PerformPartialSubstitutions,
                this.PerformMasterPageSubstitutions,
            };
        }

        /// <summary>
        /// Renders a template
        /// </summary>
        /// <param name="template">The template to render.</param>
        /// <param name="model">The model to user for rendering.</param>
        /// <param name="host">The view engine host</param>
        /// <returns>A string containing the expanded template.</returns>
        public string Render(string template, dynamic model, IViewEngineHost host)
        {
            var output = 
                this.processors.Aggregate(template, (current, processor) => processor(current, model ?? new object(), host));

            return this.matchers.Aggregate(output, (current, extension) => extension.Invoke(current, model, host));
        }

        /// <summary>
        /// <para>
        /// Gets a property value from the given model.
        /// </para>
        /// <para>
        /// Anonymous types, standard types and ExpandoObject are supported.
        /// Arbitrary dynamics (implementing IDynamicMetaObjectProvicer) are not, unless
        /// they also implement IDictionary string, object for accessing properties.
        /// </para>
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name to evaluate.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was sucessful, Item2 being the value.</returns>
        /// <exception cref="ArgumentException">Model type is not supported.</exception>
        private static Tuple<bool, object> GetPropertyValue(object model, string propertyName)
        {
            if (model == null || string.IsNullOrEmpty(propertyName))
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
                properties.Where(p => string.Equals(p.Name, propertyName, StringComparison.InvariantCulture)).
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

            foreach (var parameter in parameters)
            {
                var currentResult = GetPropertyValue(currentObject, parameter);

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
        /// <param name="nullCheck">Whether to check for null, rather than straight boolean</param>
        /// <returns>Bool representing the predicate result</returns>
        private static bool GetPredicateResult(object item, IEnumerable<string> properties, bool nullCheck)
        {
            var substitutionObject = GetPropertyValueFromParameterCollection(item, properties);

            if (substitutionObject.Item1 == false && properties.Last().StartsWith("Has"))
            {
                var newProperties =
                    properties.Take(properties.Count() - 1).Concat(new[] { properties.Last().Substring(3) });

                substitutionObject = GetPropertyValueFromParameterCollection(item, newProperties);

                return GetHasPredicateResultFromSubstitutionObject(substitutionObject.Item2);
            }

            return GetPredicateResultFromSubstitutionObject(substitutionObject.Item2, nullCheck);
        }

        /// <summary>
        /// Returns the predicate result if the substitionObject is a valid bool
        /// </summary>
        /// <param name="substitutionObject">The substitution object.</param>
        /// <param name="nullCheck"></param>
        /// <returns>Bool value of the substitutionObject, or false if unable to cast.</returns>
        private static bool GetPredicateResultFromSubstitutionObject(object substitutionObject, bool nullCheck)
        {
            if (nullCheck)
            {
                return substitutionObject == null;
            }

            if (substitutionObject != null && substitutionObject.GetType().GetProperty("Value") != null)
            {
                object value = ((dynamic)substitutionObject).Value;

                if (value is bool?)
                {
                    substitutionObject = value;
                }
            }

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
        /// <param name="host">View engine host</param>
        /// <returns>Template with @Model.PropertyName blocks expanded.</returns>
        private static string PerformSingleSubstitutions(string template, object model, IViewEngineHost host)
        {
            return SingleSubstitutionsRegEx.Replace(
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
                        return string.Empty;
                    }

                    return m.Groups["Encode"].Success ? host.HtmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
                });
        }

        /// <summary>
        /// Peforms single @Context.PropertyName substitutions.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with @Context.PropertyName blocks expanded.</returns>
        private static string PerformContextSubstitutions(string template, object model, IViewEngineHost host)
        {
            return ContextSubstitutionsRegEx.Replace(
                template,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var substitution = GetPropertyValueFromParameterCollection(host.Context, properties);

                    if (!substitution.Item1)
                    {
                        return "[ERR!]";
                    }

                    if (substitution.Item2 == null)
                    {
                        return string.Empty;
                    }

                    return m.Groups["Encode"].Success ? host.HtmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
                });
        }

        /// <summary>
        /// Performs @Each.PropertyName substitutions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with @Each.PropertyName blocks expanded.</returns>
        private static string PerformEachSubstitutions(string template, object model, IViewEngineHost host)
        {
            return EachSubstitutionRegEx.Replace(
                template,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var modelSource = GetCaptureGroupValues(m, "ModelSource").SingleOrDefault();

                    if (modelSource != null && modelSource.Equals("Context", StringComparison.OrdinalIgnoreCase))
                    {
                        model = host.Context;
                    }

                    var substitutionObject = GetPropertyValueFromParameterCollection(model, properties);

                    if (substitutionObject.Item1 == false)
                    {
                        return "[ERR!]";
                    }

                    if (substitutionObject.Item2 == null)
                    {
                        return string.Empty;
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
                        var postConditionalResult = PerformConditionalSubstitutions(contents, item, host);
                        result += ReplaceCurrentMatch(postConditionalResult, item, host);
                    }

                    return result;
                });
        }

        /// <summary>
        /// Expand a @Current match inside an @Each iterator
        /// </summary>
        /// <param name="contents">Contents of the @Each block</param>
        /// <param name="item">Current item from the @Each enumerable</param>
        /// <param name="host">View engine host</param>
        /// <returns>String result of the expansion of the @Each.</returns>
        private static string ReplaceCurrentMatch(string contents, object item, IViewEngineHost host)
        {
            return EachItemSubstitutionRegEx.Replace(
                contents,
                eachMatch =>
                {
                    if (string.IsNullOrEmpty(eachMatch.Groups["ParameterName"].Value))
                    {
                        return eachMatch.Groups["Encode"].Success ? host.HtmlEncode(item.ToString()) : item.ToString();
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

                    return eachMatch.Groups["Encode"].Success ? host.HtmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
                });
        }

        /// <summary>
        /// Performs @If.PropertyName and @IfNot.PropertyName substitutions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with @If.PropertyName @IfNot.PropertyName blocks removed/expanded.</returns>
        private static string PerformConditionalSubstitutions(string template, object model, IViewEngineHost host)
        {
            var result = template;

            result = ConditionalSubstitutionRegEx.Replace(
                result,
                m =>
                {
                    var properties = GetCaptureGroupValues(m, "ParameterName");

                    var modelSource = GetCaptureGroupValues(m, "ModelSource").SingleOrDefault();

                    if (modelSource != null && modelSource.Equals("Context", StringComparison.OrdinalIgnoreCase))
                    {
                        model = host.Context;
                    }

                    var predicateResult = GetPredicateResult(model, properties, m.Groups["Null"].Value == "Null");

                    if (m.Groups["Not"].Value == "Not")
                    {
                        predicateResult = !predicateResult;
                    }

                    return predicateResult ? m.Groups["Contents"].Value : string.Empty;
                });

            return result;
        }

        /// <summary>
        /// Perform path expansion substitutions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with paths expanded</returns>
        private static string PerformPathSubstitutions(string template, object model, IViewEngineHost host)
        {
            var result = template;

            result = PathExpansionRegEx.Replace(
                result,
                m =>
                {
                    var path = m.Groups["Path"].Value;

                    return host.ExpandPath(path);
                });

            return result;
        }

        /// <summary>
        /// Perform CSRF anti forgery token expansions
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with anti forgery tokens expanded</returns>
        private static string PerformAntiForgeryTokenSubstitutions(string template, object model, IViewEngineHost host)
        {
            return AntiForgeryTokenRegEx.Replace(template, x => host.AntiForgeryToken());
        }

        /// <summary>
        /// Perform @Partial partial view expansion
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with partials expanded</returns>
        private string PerformPartialSubstitutions(string template, object model, IViewEngineHost host)
        {
            var result = template;

            result = PartialSubstitutionRegEx.Replace(
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

                    var partialTemplate = host.GetTemplate(partialViewName, partialModel);

                    return this.Render(partialTemplate, partialModel, host);
                });

            return result;
        }

        /// <summary>
        /// Invokes the master page rendering with current sections if necessary
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with master page applied and sections substituted</returns>
        private string PerformMasterPageSubstitutions(string template, object model, IViewEngineHost host)
        {
            var masterPageName = GetMasterPageName(template);

            if (string.IsNullOrWhiteSpace(masterPageName))
            {
                return template;
            }

            var masterTemplate = host.GetTemplate(masterPageName, model);
            var sectionMatches = SectionContentsRegEx.Matches(template);
            var sections = sectionMatches.Cast<Match>().ToDictionary(sectionMatch => sectionMatch.Groups["SectionName"].Value, sectionMatch => sectionMatch.Groups["SectionContents"].Value);

            return this.RenderMasterPage(masterTemplate, sections, model, host);
        }

        /// <summary>
        /// Renders a master page - does a normal render then replaces any section tags with sections passed in
        /// </summary>
        /// <param name="masterTemplate">The master page template</param>
        /// <param name="sections">Dictionary of section contents</param>
        /// <param name="model">The model.</param>
        /// <param name="host">View engine host</param>
        /// <returns>Template with the master page applied and sections substituted</returns>
        private string RenderMasterPage(string masterTemplate, IDictionary<string, string> sections, object model, IViewEngineHost host)
        {
            var result = this.Render(masterTemplate, model, host);

            result = SectionDeclarationRegEx.Replace(
                result,
                m =>
                {
                    var sectionName = m.Groups["SectionName"].Value;

                    return sections.ContainsKey(sectionName) ? sections[sectionName] : string.Empty;
                });

            return result;
        }

        /// <summary>
		/// Gets the master page name, if one is specified
        /// </summary>
        /// <param name="template">The template</param>
        /// <returns>Master page name or String.Empty</returns>
        private static string GetMasterPageName(string template)
        {
            using (var stringReader = new StringReader(template))
            {
                var firstLine = stringReader.ReadLine();

                if (firstLine == null)
                {
                    return string.Empty;
                }

                var masterPageMatch = MasterPageHeaderRegEx.Match(firstLine);

                return masterPageMatch.Success ? masterPageMatch.Groups["MasterPage"].Value : string.Empty;
            }
        }
    }
}