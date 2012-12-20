using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nancy.ViewEngines.Razor
{
    public class TextResourceFinder: DynamicObject
    {
        private readonly ITextResource textResource;

        public TextResourceFinder(ITextResource textResource)
        {
            this.textResource = textResource;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.textResource[binder.Name];
            return true;
        }
    }
}
