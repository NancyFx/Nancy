using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Security
{
    public interface IUserIdentity
    {
        string UserName { get; set; }
        IEnumerable<string> Claims { get; set; } 
    }
}
