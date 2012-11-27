namespace Nancy.Session
{
    using System.Collections.Generic;
    using System.Dynamic;

    public class DynamicSession : DynamicDictionary
    {
        private bool hasChanged = false;

        public bool HasChanged { get { return hasChanged; } }

        public DynamicSession() { }

        public DynamicSession(IDictionary<string, dynamic> items)
        {
            foreach (var item in items)
            {
                base[item.Key] = item.Value;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            hasChanged = true;
            return base.TrySetMember(binder, value);
        }

        public override dynamic this[string name]
        {
            get
            {
                return base[name];
            }
            set
            {
                hasChanged = true;
                base[name] = value;
            }
        }

        public override void Clear()
        {
            hasChanged = true;
            base.Clear();
        }

        public override bool Remove(string key)
        {
            hasChanged = true;
            return base.Remove(key);
        }

        public override bool Remove(KeyValuePair<string, dynamic> item)
        {
            hasChanged = true;
            return base.Remove(item);
        }
    }
}
