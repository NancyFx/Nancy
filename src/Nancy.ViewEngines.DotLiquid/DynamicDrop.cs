namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using dl = global::DotLiquid;

    public class DynamicDrop : dl.Drop
    {
        private readonly dynamic _model;

        public DynamicDrop(dynamic model)
        {
            _model = model;
        }

        public override object BeforeMethod(string propertyName)
        {
            if (_model == null) return "[Model is null.]";
            var property = _model.GetType().GetProperty(propertyName);
            if (property == null) return string.Format("[Can't find :{0} in the model.]", propertyName);
            return property.GetValue(_model, null);
        }
    }
}