namespace Nancy.Demo.Hosting.AspnetCore
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get("/", args => "ASP.NET Core hosted Nancy");
		}
	}
}