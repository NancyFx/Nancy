using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Cookies
{
    public interface ICookieConfiguration
    {
        /// <summary>
        /// Cookie name for storing session information
        /// </summary>
        string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the domain of the session cookie
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the session cookie
        /// </summary>
        string Path { get; set; }
    }
}
