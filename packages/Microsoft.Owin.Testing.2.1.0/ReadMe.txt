This package provides helper classes for unit testing OWIN components.

The primary class is the TestServer, used to create an OWIN request processing pipeline and submit requests.
These requests are processed directly in memory without going over the network.

The following example creates a TestServer, adds some middleware to the OWIN pipeline, and submits a request using HttpClient:

            using(var server = TestServer.Create(app =>
                {
                    app.UseErrorPage(); // See Microsoft.Owin.Diagnostics
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync("Hello world using OWIN TestServer");
                    });
                }))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/");
                // TODO: Validate response
            }

Requests can also be constructed and submitted with the following helper methods:

            HttpResponseMessage response = await server.CreateRequest("/")
                                           .AddHeader("header1", "headervalue1")
                                           .GetAsync();
