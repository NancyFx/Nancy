using CompilerException = Spark.Compiler.CompilerException;

namespace Nancy.ViewEngines.Spark
{
    using System.Collections.Generic;

    public class SparkViewEngineResult
    {
        public SparkViewEngineResult(NancySparkView view, SparkViewEngineWrapper engine)
        {
            View = view;
            Engine = engine;
        }

        public SparkViewEngineResult(List<string> searchedLocations)
        {
            var locations = string.Empty;
            searchedLocations.ForEach(loc => locations += string.Format("{0} ", loc));

            if (!string.IsNullOrEmpty(locations))
            {
                throw new CompilerException(string.Format("The view could not be in any of the following locations: {0}", locations));
            }
        }

        public NancySparkView View { get; set; }

        public SparkViewEngineWrapper Engine { get; set; }
    }
}