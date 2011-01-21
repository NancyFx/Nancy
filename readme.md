# Meet Nancy

Nancy is a lightweight web framework for the .Net platform, inspired by Sinatra. Nancy aims to deliver a low ceremony approach to building light, fast web applications.

## Features

* Built from the bottom up, not simply a DSL on top of an existing framework. Removing limitations and feature hacks of an underlying framework, as well as the need to reference more assemblies than you need. _keep it light_
* Abstracted away from ASP.NET / IIS so that it can run on multiple hosting environments (planned [OWIN](http://bvanderveen.com/a/dotnet-http-abstractions "Read more about the Open Web Interface for .NET") support), such as (but not limited to) ASP.NET, WCF, Mono/FastCGI and more (ASP.NET and WCF currently supported out-of-the-box)
* Ultra lightweight action declarations for GET, HEAD, PUT, POST and DELETE requests
* View engine integration
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

## Bootstrappers

The bootstrapper projects for third party IoC containers are only temporarily in the source code. These will be removed when the IoC integration design has been proven stable. They will be moving into a contrib-style project and/or the Nancy wiki. The reason they
won't ship with Nancy is because we do not want to be tasked each time there is a new version of the containers released.

## Community

You can find lot of Nancy users on the [Nancy User Group](https://groups.google.com/forum/?fromgroups#forum/nancy-web-framework). That is were most of the discussions regarding the development and usage of Nancy is taking place. You can also
find Nancy on Twitter using the #NancyFx hashtag.	
	
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

## View Engines

There is a rich set of view engines in the .net space and most of them have been design in such a away that they are framework agnostic, all that is needed is a bit
of integration work to get it running in Nancy. Currently there are support for [Razor](http://weblogs.asp.net/scottgu/archive/2010/07/02/introducing-razor.aspx "Read more about the Razor view engine"), 
[Spark](http://sparkviewengine.com "Read more about the Spark view engine"), [NDjango](http://ndjango.org "Read more about the NDjango view engine") and a static file engine. There used to be an [NHaml](http://code.google.com/p/nhaml "Read more about the NHaml view engine")
engine but it was removed since the view engine project appears to have come to a dead halt. Let us know if that change! WebForms engine.. well let's just say that if you have a need to use WebForms with Nancy then _we accept patches_!

All view engine integrations are still very early implementations and more work will be put towards them to bring as many of the features, as possible, into Nancy! If you have any experience with either of the engines, grab a fork and start coding!

## Contributors

Nancy is not a one man project and many of the features that are availble would not have been possible without the awesome contributions from the community!

* Andy Pike
* Graeme Foster
* Ian Davis
* Jonas Cannehag
* José F. Romaniello
* Karl Seguin
* James Eggers
* Jason Mead
* Jeremy Skinner
* João Bragança
* Johan Danforth
* John Downey
* Mindaugas Mozuras
* Pedro Felix
* Piotr Wlodek
* Phil Haack
* Robert Greyling
* Steven Robbins
* Vidar L. Sømme

## Copyright

Copyright © 2010 Andreas Håkansson

## License

Nancy is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.