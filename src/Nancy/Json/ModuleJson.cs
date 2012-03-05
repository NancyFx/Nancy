using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Json
{
	public static class ModuleJson
	{
		public static void DoNotCacheJsonResponse(this NancyModule module)
		{
			module.After.AddItemToEndOfPipeline(ctx =>
			{
				if (Json.IsJsonContentType(ctx.Response.ContentType))
				{
					ctx.Response.DoNotCache();
				}
			});
		}
	}
}
