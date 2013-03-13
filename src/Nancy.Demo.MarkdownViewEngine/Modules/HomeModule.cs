namespace Nancy.Demo.MarkdownViewEngine.Modules
{
    using Model;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ =>
                           {
                               var model = new MainModel(
                                   "Jimbo",
                                   new[]
                                       {new User("Bob", "Smith"), new User("Jimbo", "Jones"), new User("Bill", "Bobs"),},
                                   "<script type=\"text/javascript\">alert('Naughty JavaScript!');</script>");

                               return View["Home", model];
                           };
        }
    }
}