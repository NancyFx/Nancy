using System;

namespace Nancy.ViewEngines.Razor
{
	public class ViewRenderException : Exception
	{
		public ViewRenderException(string msg) : base(msg)
		{
		}
	}
}