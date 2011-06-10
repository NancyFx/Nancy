namespace Nancy.ViewEngines.DotLiquid
{
    using global::DotLiquid;

    public class DynamicDrop : Drop
    {
        private readonly dynamic model;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDrop"/> class.
        /// </summary>
        /// <param name="model"></param>
        public DynamicDrop(dynamic model)
        {
            this.model = model;
        }

        public override object BeforeMethod(string propertyName)
        {
            if (model == null)
            {
                return "[Model is null.]";
            }

            var property = model.GetType().GetProperty(propertyName);

            return property == null ? 
                string.Format("[Can't find :{0} in the model.]", propertyName) : 
                property.GetValue(model, null);
        }
    }
}