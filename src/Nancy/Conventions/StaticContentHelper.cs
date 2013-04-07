using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Conventions
{
    /* Example Usage */
    //public class Boostrapper : DefaultNancyBootstrapper
    //{
    //    protected override void ConfigureConventions(NancyConventions conventions)
    //    {
    //        base.ConfigureConventions(conventions);

    //        conventions.Static((File, Directory) =>
    //        {
    //            File["/page.js"] = "page.min.js";
    //            File["/page.css"] = "page.min.css";

    //            Directory["/images"] = "images";
    //        });
    //    }
    //}

    public static class StaticContentHelper
    {
        public static void Static(this NancyConventions conventions, Action<StaticFileContent, StaticDirectoryContent> staticConventions)
        {
            staticConventions(new StaticFileContent(conventions), new StaticDirectoryContent(conventions));
        }
    }
}
