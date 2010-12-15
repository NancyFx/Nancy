using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spark;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace Nancy.ViewEngines.Spark.Descriptors
{
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

        #region IDescriptorBuilder Members

        public virtual IDictionary<string, object> GetExtraParameters(ActionContext actionContext)
        {
            var extra = new Dictionary<string, object>();
            foreach (IDescriptorFilter filter in Filters)
                filter.ExtraParameters(actionContext, extra);
            return extra;
        }

        public virtual SparkViewDescriptor BuildDescriptor(BuildDescriptorParams buildDescriptorParams, ICollection<string> searchedLocations)
        {
            var descriptor = new SparkViewDescriptor
                                 {
                                     TargetNamespace = buildDescriptorParams.ViewPath
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

            string trailingUseMaster = TrailingUseMasterName(descriptor);
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

        #endregion

        public virtual void Initialize(ISparkServiceContainer container)
        {
            engine = container.GetService<ISparkViewEngine>();
            grammar = new UseMasterGrammar(engine.Settings.Prefix);
        }

        public string TrailingUseMasterName(SparkViewDescriptor descriptor)
        {
            string lastTemplate = descriptor.Templates.Last();
            SourceContext sourceContext = AbstractSyntaxProvider.CreateSourceContext(lastTemplate, engine.ViewFolder);
            if (sourceContext == null)
                return null;
            ParseResult<string> result = ParseUseMaster(new Position(sourceContext));
            return result == null ? null : result.Value;
        }

        private bool LocatePotentialTemplate(
            IEnumerable<string> potentialTemplates,
            ICollection<string> descriptorTemplates,
            ICollection<string> searchedLocations)
        {
            string template = potentialTemplates.FirstOrDefault(t => engine.ViewFolder.HasView(t));
            if (template != null)
            {
                descriptorTemplates.Add(template);
                return true;
            }
            if (searchedLocations != null)
            {
                foreach (string potentialTemplate in potentialTemplates)
                    searchedLocations.Add(potentialTemplate);
            }
            return false;
        }

        private IEnumerable<string> ApplyFilters(IEnumerable<string> locations, IDictionary<string, object> extra)
        {
            // apply all of the filters PotentialLocations in order
            return Filters.Aggregate(
                locations,
                (aggregate, filter) => filter.PotentialLocations(aggregate, extra));
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

        #region Nested type: UseMasterGrammar

        /// <summary>
        /// Simplified parser for &lt;use master=""/&gt; detection.
        /// TODO: Rob G - move somewhere else when I've had some sleep - probably to Spark.Parser in Core
        /// </summary>
        private class UseMasterGrammar : CharGrammar
        {
            public UseMasterGrammar(string prefix)
            {
                ParseAction<IList<char>> whiteSpace0 = Rep(Ch(char.IsWhiteSpace));
                ParseAction<IList<char>> whiteSpace1 = Rep1(Ch(char.IsWhiteSpace));
                ParseAction<string> startOfElement = !string.IsNullOrEmpty(prefix) ? Ch("<" + prefix + ":use") : Ch("<use");
                ParseAction<Chain<Chain<Chain<string, IList<char>>, char>, IList<char>>> startOfAttribute = Ch("master").And(whiteSpace0).And(Ch('=')).And(whiteSpace0);
                ParseAction<Chain<Chain<char, IList<char>>, char>> attrValue = Ch('\'').And(Rep(ChNot('\''))).And(Ch('\''))
                    .Or(Ch('\"').And(Rep(ChNot('\"'))).And(Ch('\"')));

                ParseAction<string> endOfElement = Ch("/>");

                ParseAction<string> useMaster = startOfElement
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

        #endregion
    }
}