# Meet Nancy

Nancy is a lightweight web framework for the .Net platform, inspired by Sinatra. Nancy aims to deliver a low ceremony approach to building light, fast web applications. 

## Features

* Built from the bottom up, not simply a DSL on top of an existing framework. Removing limitations and feature hacks of an underlying framework, as well as the need to reference more assemblies than you need. _keep it light_
* Abstracted away from ASP.NET / IIS so that it can run on multiple hosting environments (see below for planned OWIN support), such as (but not limited to) ASP.NET, WCF, Mono/FastCGI and more (ASP.NET and WCF currently supported)
* Ultra lightweight action declarations for GET, PUT, POST and DELETE requests
* View engine integration (Spark and Razor in development, read below how to help add more to the list)
* Powerful request path matching that includes advanced parameter capabilities. The path matching strategy can be replaced with custom implementations to fit your exact needs
* Easy response syntax, enabling you to return things like int, string, HttpStatusCode and Action<Stream> elements without having to explicitly cast or wrap your response - you just return it and Nancy _will_ do the work for you

## Usage

Set up your web.config file:

    <httpHandlers>
        <add verb="*" type="Nancy.Hosting.NancyHttpRequestHandler" path="*"/>
    </httpHandlers>
    
    <system.webServer>
        <validation validateIntegratedModeConfiguration="false"/>
        <handlers>
            <add name="Nancy" verb="*" type="Nancy.Hosting.NancyHttpRequestHandler" path="*"/>
        </handlers>
    </system.webServer>

Start adding your Nancy modules containing your actions:
	
    public class Module : NancyModule
    {
        public Module()
        {
            Get["/"] = x => {
                return "This is the root";
            };
        }
    }

Start your application and enjoy! Swap out Get with either Put, Post or Delete to create actions that will respond to calls using those request methods. 

If you want to get fancy you can add parameters to your paths:

    public class Module : NancyModule
    {
        public Module()
        {
            Get["/greet/{name}"] = x => {
                return string.Concat("Hello ", x.name);
            };
        }
    }

The _{name}_ parameter will be captured and injected into the action parameters, shown as _x_ in the sample. The parameters are represented by a _dynamic_ type so you can access any parameter name straight on it as a property or an indexer. For more information on action parameters please refer to the [Nancy introduction post](http://elegantcode.com/2010/11/28/introducing-nancy-a-lightweight-web-framework-inspired-by-sinatra "Read the Nancy introduction post at elegantcode.com") over at my blog on [ElegantCode](http://elegantcode.com "Visit ElegantCode).

Nancy also supports the idea of _module paths_, where you assign a root path for all actions in the module and they will all be relative to that:

    public class Module : NancyModule
    {
        public Module() : base("/butler")
        {
            Get["/greet/{name}"] = x => {
                return string.Concat("Hello ", x.name);
            };
        }
    }

Notice the _base("/butler")_ call to the NancyModule constructor. Now all action paths that are defined in the module will be relative to _/butler_ so in order to greet someone you could access _/butler/greet/{name}_, for example _/butler/greet/thecodejunkie_
	
## Help out

There are many ways you can contribute to Nancy. Like most open-source software projects, contributing code
is just one of many outlets where you can help improve. Some of the things that you could help out with in
Nancy are:

* Documentation (both code and features)
* Bug reports
* Bug fixes
* Feature requests
* Feature implementations
* Test coverage
* Code quality
* Sample applications

## TODO / Design decisions

1. Make Nancy run on the [Open Web Interface for .NET](http://bvanderveen.com/a/dotnet-http-abstractions "Read more about the Open Web Interface for .NET")
2. Enable IoC container integration so that Nancy modules can have dependencies that are resolved at runtime
3. The ability to wire up your favorite IoC container with Nancy and use it to resolve module dependencies
4. Ship a nice set of Response formatters, such as json, xml and others
5. Request and Response interception to enable rich middleware capabilities such as caching and logging
6. View engine integration. Spark and Razor are planned. Looking for contributors for Haml, NDjango and other popular view engines (contact me if you want to help out!)
7. Self-composing framework - make Nancy use an internal IoC to compose the framework at runtime. Increasing modularity of the framework and the ability to swap out parts
8. NuGet presence

## Contributors

* Graeme Foster
* Jason Mead
* Joao Braganca
* John Downey
* Pedro Felix

## Copyright

Copyright © 2010 Andreas Håkansson

## License

Nancy is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.