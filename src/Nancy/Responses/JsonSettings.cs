namespace Nancy.Responses
{
    /// <summary>
    /// Json serializer settings
    /// </summary>
    public static class JsonSettings
    {
        /// <summary>
        /// Max length of json output
        /// </summary>
        public static int MaxJsonLength { get; set; }

        /// <summary>
        /// Maximum number of recursions
        /// </summary>
        public static int MaxRecursions { get; set; }

        static JsonSettings()
        {
            MaxJsonLength = 102400;
            MaxRecursions = 100;
        }
    }
}