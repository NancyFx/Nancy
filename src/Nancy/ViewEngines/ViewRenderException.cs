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

		/// <summary>
		/// Create an instance of <see cref="ViewRenderException"/>
		/// </summary>
		/// <param name="msg">A description of the rendering problem</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public ViewRenderException(string msg, Exception innerException) : base(msg, innerException)
		{
		}
	}
}