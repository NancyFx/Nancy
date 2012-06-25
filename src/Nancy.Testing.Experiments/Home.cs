namespace Nancy.Testing.Experiments
{
    using Models;
    using Nancy;

    public class Home : NancyModule
    {
        public Home(IModelFactory modelFactory)
        {
            Get["/"] = parameters => {

                var model =
                    modelFactory.GetModel("This is a message from the model");

                return View["index"];
            };
        }
    }
}