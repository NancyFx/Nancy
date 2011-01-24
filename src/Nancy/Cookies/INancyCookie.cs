namespace Nancy.Cookies
{
    using System;

    public interface INancyCookie
    {
        /// <summary>
        /// The name of the cookie
        /// </summary>
        string Name { get; } 
        /// <summary>
        /// The value of the cookie
        /// </summary>
        string Value { get;  }
        /// <summary>
        /// The domain to restrict the cookie to
        /// </summary>
        string Domain { get; set; }
        /// <summary>
        /// The path to restrict the cookie to
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// When the cookie should expire
        /// </summary>
        DateTime? Expires { get; set; }
    }
}