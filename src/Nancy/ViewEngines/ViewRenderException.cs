namespace Nancy.ViewEngines.Razor
{
	using System;

	/// <summary>
	/// An exception that indicates the view could not be rendered
	/// </summary>
	public class ViewRenderException : Exception
	{
		/// <summary>
		/// Create an instance of <see cref="ViewRenderException"/>
		/// </summary>
		/// <param name="msg">A description of the rendering problem</param>
		public ViewRenderException(string msg) : base(msg)
		{
		}
	}
}