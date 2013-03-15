namespace Nancy.Demo.MarkdownViewEngine.Modules
{
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using MarkdownSharp;
	using ViewEngines;
	
	public class HomeModule : NancyModule
	{
		private readonly IViewLocationProvider viewLocationProvider;
		
		public HomeModule(IViewLocationProvider viewLocationProvider, IViewLocator viewLocator, IFileSystemReader fileSystemReader, IRootPathProvider rootPathProvider)
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
				var markdown = x.Contents().ReadToEnd();
				return new BlogModel(markdown);
			})
				.OrderByDescending(x => x.BlogDate)
					.ToList();
			
			return model;
		}
	}
}
