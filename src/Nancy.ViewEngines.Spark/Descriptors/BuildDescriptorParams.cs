namespace Nancy.ViewEngines.Spark.Descriptors
{
    using System.Collections.Generic;
    using System.Linq;

    public class BuildDescriptorParams
    {
        private static readonly IDictionary<string, object> extraEmpty = new Dictionary<string, object>();
        private readonly IDictionary<string, object> extra;
        private readonly bool findDefaultMaster;
        private readonly int hashCode;
        private readonly string masterName;
        private readonly string viewName;
        private readonly string viewPath;

        public BuildDescriptorParams(string viewPath, string viewName, string masterName, bool findDefaultMaster, IDictionary<string, object> extra)
        {
            this.viewPath = viewPath;
            this.viewName = viewName;
            this.masterName = masterName;
            this.findDefaultMaster = findDefaultMaster;
            this.extra = extra ?? extraEmpty;

            // this object is meant to be immutable and used in a dictionary.
            // the hash code will always be used so it isn't calculated just-in-time.
            hashCode = CalculateHashCode();
        }

        public string ViewPath
        {
            get { return viewPath; }
        }

        public string ViewName
        {
            get { return viewName; }
        }

        public string MasterName
        {
            get { return masterName; }
        }

        public bool FindDefaultMaster
        {
            get { return findDefaultMaster; }
        }

        public IDictionary<string, object> Extra
        {
            get { return extra; }
        }

        private static int Hash(object str)
        {
            return str == null ? 0 : str.GetHashCode();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        private int CalculateHashCode()
        {
            return Hash(viewName) ^
                   Hash(viewPath) ^
                   Hash(masterName) ^
                   findDefaultMaster.GetHashCode() ^
                   extra.Aggregate(0, (hash, kv) => hash ^ Hash(kv.Key) ^ Hash(kv.Value));
        }

        public override bool Equals(object obj)
        {
            var that = obj as BuildDescriptorParams;

            if (that == null || that.GetType() != GetType())
            {
                return false;
            }

            if (!string.Equals(viewName, that.viewName) ||
                !string.Equals(viewPath, that.viewPath) ||
                !string.Equals(masterName, that.masterName) ||
                findDefaultMaster != that.findDefaultMaster ||
                extra.Count != that.extra.Count)
            {
                return false;
            }

            foreach (var kv in extra)
            {
                object value;
                if (!that.extra.TryGetValue(kv.Key, out value) || !Equals(kv.Value, value))
                {
                    return false;
                }

            }

            return true;
        }
    }
}