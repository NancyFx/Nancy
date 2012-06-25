namespace Models
{
    public class DefaultModelFactory : IModelFactory
    {
        public DefaultModelFactory()
        {
        }

        public Model GetModel(string message)
        {
            return new Model(message);
        }
    }

    public interface IModelFactory
    {
        Model GetModel(string message);
    }

    public class Model
    {
        public Model(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }
    }
}
