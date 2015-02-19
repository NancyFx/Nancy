namespace Nancy.Demo.MarkdownViewEngine.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using Nancy.ViewEngines;

    public class HomeModule : NancyModule
    {
        private readonly IViewLocationProvider viewLocationProvider;

        public HomeModule(IViewLocationProvider viewLocationProvider)
        {
            this.viewLocationProvider = viewLocationProvider;

            Get["/"] = _ =>
            {
                var popularposts = GetModel();

                dynamic postModel = new ExpandoObject();
                postModel.PopularPosts = popularposts;
                postModel.MetaData = popularposts;

                return View["blogindex", postModel];
            };

            Get["/{viewname}"] = parameters =>
            {
                var popularposts = GetModel();

                dynamic postModel = new ExpandoObject();
                postModel.PopularPosts = popularposts;
                postModel.MetaData =
                popularposts.FirstOrDefault(x => x.Slug == parameters.viewname);

                return View["Posts/" + parameters.viewname, postModel];
            };
        }

        private IEnumerable<BlogModel> GetModel()
        {
            var views = this.viewLocationProvider.GetLocatedViews(new[] { "md", "markdown" });
            var model = views.Select(x =>
            {
                using (var reader = x.Contents())
                {
                    var markdown = reader.ReadToEnd();
                    return new BlogModel(markdown);
                }
            })
            .Where(x => x.BlogDate.Date <= DateTime.Today) //Allow for future posts to be lined up but don't show
            .OrderByDescending(x => x.BlogDate)
            .ToList();

            return model;
        }
    }
}
