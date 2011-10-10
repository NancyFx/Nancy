namespace Nancy.ViewEngines.Spark.Descriptors
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Spark;
    using global::Spark;
    using global::Spark.Parser;
    using global::Spark.Parser.Syntax;

    public class DefaultDescriptorBuilder : IDescriptorBuilder
    {
        private ISparkViewEngine engine;
        private UseMasterGrammar grammar;

        public DefaultDescriptorBuilder()
            : this((string) null)
        {
        }

        public DefaultDescriptorBuilder(string prefix)
        {
            Filters = new List<IDescriptorFilter>();
            grammar = new UseMasterGrammar(prefix);
        }

        public DefaultDescriptorBuilder(ISparkViewEngine engine)
            : this()
        {
            this.engine = engine;
            grammar = new UseMasterGrammar(this.engine.Settings.Prefix);
        }

        public IList<IDescriptorFilter> Filters { get; set; }

        public ParseAction<string> ParseUseMaster
        {
            get { return grammar.ParseUseMaster; }
        }

        public virtual IDictionary<string, object> GetExtraParameters(ViewLocationResult viewLocationResult)
        {
            var extra = new Dictionary<string, object>();
            foreach (var filter in Filters)
            {
                filter.ExtraParameters(viewLocationResult, extra);
            }

            return extra;
        }

        public virtual SparkViewDescriptor BuildDescriptor(BuildDescriptorParams buildDescriptorParams, ICollection<string> searchedLocations)
        {
            var descriptor = new SparkViewDescriptor
                                 {
                                     TargetNamespace = GetNamespaceEncodedPathViewPath(buildDescriptorParams.ViewPath)
                                 };

            if (!LocatePotentialTemplate(
                PotentialViewLocations(buildDescriptorParams.ViewPath,
                                       buildDescriptorParams.ViewName,
                                       buildDescriptorParams.Extra),
                descriptor.Templates,
                searchedLocations))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(buildDescriptorParams.MasterName))
            {
                if (!LocatePotentialTemplate(
                    PotentialMasterLocations(buildDescriptorParams.MasterName,
                                             buildDescriptorParams.Extra),
                    descriptor.Templates,
                    searchedLocations))
                {
                    return null;
                }
            }
            else if (buildDescriptorParams.FindDefaultMaster && TrailingUseMasterName(descriptor) == null /*empty is a valid value*/)
            {
                LocatePotentialTemplate(
                    PotentialDefaultMasterLocations(buildDescriptorParams.ViewPath,
                                                    buildDescriptorParams.Extra),
                    descriptor.Templates,
                    null);
            }

            var trailingUseMaster = TrailingUseMasterName(descriptor);
            while (buildDescriptorParams.FindDefaultMaster && !string.IsNullOrEmpty(trailingUseMaster))
            {
                if (!LocatePotentialTemplate(
                    PotentialMasterLocations(trailingUseMaster,
                                             buildDescriptorParams.Extra),
                    descriptor.Templates,
                    searchedLocations))
                {
                    return null;
                }
                trailingUseMaster = TrailingUseMasterName(descriptor);
            }

            return descriptor;
        }

        public virtual void Initialize(ISparkServiceContainer container)
        {
            engine = container.GetService<ISparkViewEngine>();
            grammar = new UseMasterGrammar(engine.Settings.Prefix);
        }

        public string TrailingUseMasterName(SparkViewDescriptor descriptor)
        {
            var lastTemplate = descriptor.Templates.Last();
            var sourceContext = AbstractSyntaxProvider.CreateSourceContext(lastTemplate, engine.ViewFolder);

            if (sourceContext == null)
            {
                return null;
            }

            var result = ParseUseMaster(new Position(sourceContext));

            return result == null ? null : result.Value;
        }

        private bool LocatePotentialTemplate(
            IEnumerable<string> potentialTemplates,
            ICollection<string> descriptorTemplates,
            ICollection<string> searchedLocations)
        {
            var template = potentialTemplates.FirstOrDefault(t => engine.ViewFolder.HasView(t));
            if (template != null)
            {
                descriptorTemplates.Add(template);
                return true;
            }

            if (searchedLocations != null)
            {
                foreach (var potentialTemplate in potentialTemplates)
                {
                    searchedLocations.Add(potentialTemplate);
                }
            }

            return false;
        }

        /// <remarks>Apply all of the filters PotentialLocations in order</remarks>
        private IEnumerable<string> ApplyFilters(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            return Filters.Aggregate(locations, (aggregate, filter) => filter.PotentialLocations(aggregate, extra));
        }

        protected virtual IEnumerable<string> PotentialViewLocations(string viewPath, string viewName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        Path.Combine(viewPath, viewName + ".spark"),
                                        Path.Combine("Shared", viewName + ".spark")
                                    }, extra);
        }

        protected virtual IEnumerable<string> PotentialMasterLocations(string masterName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        Path.Combine("Layouts", masterName + ".spark"),
                                        Path.Combine("Shared", masterName + ".spark")
                                    }, extra);
        }

        protected virtual IEnumerable<string> PotentialDefaultMasterLocations(string controllerName, IDictionary<string, object> extra)
        {
            return ApplyFilters(new[]
                                    {
                                        Path.Combine("Layouts", "Application.spark"),
                                        Path.Combine("Shared", "Application.spark")
                                    }, extra);
        }

        private static string GetNamespaceEncodedPathViewPath(string viewPath)
        {
            return viewPath.Replace('\\', '_');
        }

        /// <summary>
        /// Simplified parser for &lt;use master=""/&gt; detection.
        /// TODO: Rob G - move somewhere else when I've had some sleep - probably to Spark.Parser in Core
        /// </summary>
        private class UseMasterGrammar : CharGrammar
        {
            public UseMasterGrammar(string prefix)
            {
                var whiteSpace0 = Rep(Ch(char.IsWhiteSpace));
                var whiteSpace1 = Rep1(Ch(char.IsWhiteSpace));
                var startOfElement = !string.IsNullOrEmpty(prefix) ? Ch("<" + prefix + ":use") : Ch("<use");
                var startOfAttribute = Ch("master").And(whiteSpace0).And(Ch('=')).And(whiteSpace0);
                var attrValue = Ch('\'').And(Rep(ChNot('\''))).And(Ch('\''))
                    .Or(Ch('\"').And(Rep(ChNot('\"'))).And(Ch('\"')));

                var endOfElement = Ch("/>");

                var useMaster = startOfElement
                    .And(whiteSpace1)
                    .And(startOfAttribute)
                    .And(attrValue)
                    .And(whiteSpace0)
                    .And(endOfElement)
                    .Build(hit => new string(hit.Left.Left.Down.Left.Down.ToArray()));

                ParseUseMaster =
                    pos =>
                        {
                            for (Position scan = pos; scan.PotentialLength() != 0; scan = scan.Advance(1))
                            {
                                ParseResult<string> result = useMaster(scan);
                                if (result != null)
                                    return result;
                            }
                            return null;
                        };
            }

            public ParseAction<string> ParseUseMaster { get; private set; }
        }
    }
}