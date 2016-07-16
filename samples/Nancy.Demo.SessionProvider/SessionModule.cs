using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Demo.SessionProvider
{
    public class SessionModule:NancyModule
    {
        public SessionModule()
        {
            Get<object>("something", p => {
                if (Session["test"] == null)
                {
                    Session["test"] = "abc";
                }

                return Session["test"];
            });
        }
    }
}
