# Meet Nancy Token Authentication

The Nancy.Authentication.Token project was built for use by heterogeneous clients (iOS apps, Android apps, Angular SPA apps, etc.) that all communicate with the same back-end Nancy application.

## Rationale

Token authentication and authorization was built with the following requirements:

* No Cookies (since not all client apps are web browsers)
* Avoid retrieving users and permissions from a backend data store once the user has been authenticated/authorized
* Allow client apps to store a token containing the current user's credentials for resubmission on subsequent requests (after first authenticating)
* Prevent rogue clients from simply generating their own spoofed credentials by incorporating a one-way hashing algorithm that ensures the token has not been tampered with
* Use server side keys for token hash generation with a configurable key expiration interval
* Use file system storage of server-side token generation private keys to allow keys to survive an application restart or an app pool recycle. Note: an "in memory" option is available primarily for testing, but could be used in a situation where expiring all user sessions on an application restart is acceptable behavior.

Token Authentication can be wired up in a simliar fashion to other available forms of Nancy authentication.

```csharp
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            TokenAuthentication.Enable(pipelines, new TokenAuthenticationConfiguration(container.Resolve<ITokenizer>()));
        }
    }
```

You will need to provide your own form of initial user authentication. This can use your own custom implementation that queries
from a database, from an AD store, from a webservice, or any other form you choose. It could also use another form of Nancy authentication (Basic with an IUserValidator implementation
for example).

Tokens are generated from an `IUserIdentity` and a `NancyContext` by an implementation of `ITokenizer`. The 
default implementation is named `Tokenizer` and provides some configuration options. By default, it generates a token
that includes the following components:

* User name
* Pipe separated list of user claims
* UTC now in ticks
* The client's "User-Agent" http header value (required)

It is recommended that you configure the Tokenizer to use an additional piece of information that can uniquely identify 
the client device. 

The following code shows an example of how you can perform the initial user authorization and return the generated token to the client.

```csharp
    public class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer)
            : base("/auth")
        {
            Post["/"] = x =>
                {
                    var userName = (string)this.Request.Form.UserName;
                    var password = (string)this.Request.Form.Password;

                    var userIdentity = UserDatabase.ValidateUser(userName, password);

                    if (userIdentity == null)
                    {
                        return HttpStatusCode.Unauthorized;
                    }

                    var token = tokenizer.Tokenize(userIdentity, Context);

                    return new
                        {
                            Token = token,
                        };
                };

            Get["/validation"] = _ =>
                {
                    this.RequiresAuthentication();
                    return "Yay! You are authenticated!";
                };

            Get["/admin"] = _ =>
            {
                this.RequiresAuthentication();
                this.RequiresClaims(new[] { "admin" });
                return "Yay! You are authorized!";
            };
        }
    }
```

## Contributors

Nancy.Authentication.Token was originally created by the crack development team at [Lotpath](http://lotpath.com) ([Lotpath on github](http://github.com/Lotpath)).
