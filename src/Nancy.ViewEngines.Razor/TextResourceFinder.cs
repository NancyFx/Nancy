using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nancy.ViewEngines.Razor
{
    public class TextResourceFinder : DynamicObject, ITextResourceFinder
    {
        private readonly ITextResource textResource;
        private readonly string culture;

        public TextResourceFinder(ITextResource textResource, string culture)
        {
            this.textResource = textResource;
            this.culture = culture;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.textResource[binder.Name, this.culture];
            return true;
        }
    }
}
