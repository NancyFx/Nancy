namespace Nancy.Xml
{
	using System.Text;

	public static class XmlSettings
	{
		/// <summary>
		/// Turn on or off encoding for xml responses
		/// </summary>
		public static bool EncodingEnabled { get; set; }

		/// <summary>
		/// Default encoding for xml responses
		/// </summary>
		public static Encoding DefaultEncoding { get; set; }

		static XmlSettings()
		{
			EncodingEnabled = false;
			DefaultEncoding = Encoding.UTF8;
		}
	}
}
