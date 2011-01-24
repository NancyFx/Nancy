namespace Nancy.Html5BoilerPlateTemplate.Modules
{
    using Nancy;
    using Nancy.Html5BoilerPlateTemplate.Models;
    using Nancy.Formatters;
    using Nancy.ViewEngines;


    public class MainModule : NancyModule
    {
        public MainModule()
        {

            /* Error pages */

            //TODO: Configure redirect in Nancy for 404.html

            Get["/404"] = x => {
                return View.Static("~/404.html");
            };

            /* Root page */ 
            
            Get["/"] = x => {
                return View.Static("~/index.html");
            };
            
            /* Generic folders */
            
            Get["/js/{name}"] = x =>
            {
                return View.Static(string.Concat("~/js/", (string)x.name));
            };

            Get["/css/{name}"] = x =>
            {
                return View.Static(string.Concat("~/css/" + (string) x.name));
            };

            Get["/images/{name}"] = x =>
            {
                return Response.Image(string.Concat("~/images/" + (string)x.name));
            };


            /* Generic files */

            Get["/apple-touch-icon.png"] = x =>
            {
                return Response.Image("~/apple-touch-icon.png");
            };


            Get["/favicon.ico"] = x =>
            {
                return Response.Image("~/favicon.ico");
            };


            Get["/crossdomain.xml"] = x =>
            {
                return View.Static("~/crossdomain.xml");
            };


            Get["/robots.txt"] = x =>
            {
                return View.Static("~/robots.txt");
            };
            
        }
    }
}
