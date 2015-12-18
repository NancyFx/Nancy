@Master['master']

@Tags
Date: 19/12/2012
Title: Why use Nancy
Tags: Nancy,.Net,C#
@EndTags

@Section['Content']

@Partial['blogheader', Model.MetaData];

# Why use Nancy?

When a new project comes along why should you automatically choose ASP.NET MVC?  Yes, its Microsoft based so you may have more of your peers fluent already in that architecture but is there an alternative, a better alternative?

I believe so and its called [NancyFX][1].  Your first reaction, what is so special about Nancy?  I also believe you’ll ask what is wrong with ASP.NET MVC but maybe you should look at it differently and ask what is right with Nancy?

## What is Nancy?
Nancy is a lightweight framework for building websites / services without getting in your way. It’s heavily inspired by a Ruby project called Sinatra, which happens to identify itself as not being a framework, since it doesn’t include all the plumbing of things such as an ORM, lots of configuration, etc. 

##Does it implement MVC?
Nancy does not force you to adhere to the model-view-controller pattern, or any other pattern. It’s nothing more than a service endpoint responding to HTTP verbs. Making it ideal for building Websites, Web Services and APIs.

That doesn’t mean you can’t apply the MVC pattern to Nancy. You can define Views and put them in a Views folder, create Models to return from your endpoints, and map requests to Models, just like you currently do with ASP.NET MVC. 

##Key Considerations

__Easier Testing__ - Nancy provides a testing library that allows you to test the full request/response cycle so not only can you test that your request returns the model you expect you can test that when you pass in accept headers the response is in the format you expect. For example:

    [Fact]
    public void GetData_WhenRequested_ShouldReturnOKStatusCode()
    {
        var browser = new Browser();
        var response = await browser.Get("/GetData", (with) =>
        {
            with.Header("Authorization", "Bearer johnsmith");
            with.Header("Accept", "application/json");
            with.HttpRequest();
        });
    
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

I am unaware of how you would be able to test this in MVC without it being a full integration test whereas Nancy has no dependencies on System.Web or MVC so it can provide us with a Response without hitting a server.

__Automatic Dependency Resolution__ - Nancy provides an in built IOC container called [TinyIOC][2] which will find all your dependencies automatically for you or if you want/need to configure something you can do so at various points in your application.  This is done in a Bootstrapper class that exposes various methods and properties to allow you to configure Nancy.


    protected override void ConfigureApplicationContainer(TinyIoCContainer container)
    {
        base.ConfigureApplicationContainer(container);
    
        var store = new EmbeddableDocumentStore()
        {
            ConnectionStringName = "RavenDB"
        };
    
        store.Initialize();
    
        container.Register<IDocumentStore>(store);
    }
    
    protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
    {
        base.ConfigureRequestContainer(container, context);
    
        var store = container.Resolve<IDocumentStore>();
        var documentSession = store.OpenSession();
    
        container.Register<IDocumentSession>(documentSession);
    }


Here the IOC container is used in different places within an application's lifecycle. Once at the startup and once per request.  It registers a DocumentStore which should be done only once in an application and then on every request it finds the DocumentStore and uses it to open a session and registers it with the IOC.  If you have a service that has a IDocumentSession dependency then it will come via this.

If for some reason you're being stubborn and want to use your preferred IOC container, Nancy supports all the main IOC players allowing you to register your dependencies with them instead.

__Completely Customisable, Conventions & Better Extension Points__ - One of Nancy's core features is its extensibility.  It it designed to allow you to replace any part you want.  You can have custom model binders, view renderers, serializers in fact you can implement your own INancyEngine and completely change how Nancy handles requests etc.  There are also a set of pre-defined conventions that you can swap in/out if you want Nancy to do something different than what comes as standard.  Everything is complete customisable and very easy to modify Nancy's behaviour which offers great extensibility points if you wanted to create a 3rd party library for example.

__Terse Syntax & Less Ceremony__ - Nancy provides a nice terse syntax that does not get in the way of your application and leaves you to write your code. What I have found is that due to the terse syntax it encourages you to make your application code nice and neat too.  One example of less ceremony and terseness is that you can get a full Nancy application running inside a 140 character tweet!


    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Hello World";
        }
    }


__Runs on Mono__ - Nancy does not tie itself down to Windows it works just as well on OSX and Linux under [Mono][3] which allows your team to work on multiple platforms. In fact Nancy can even run on a [Raspberry Pi][4] I would like to see ASP.NET MVC do that!

__Content Negotiation__ - Content Negotiation is built into Nancy and runs out of the box.  This means Nancy can be used in an API type application as well as a website application.  In fact if you wanted you could have it do both very easily.


    Get["/"] = parameters => {
        return Negotiate
            .WithModel(new RatPack {FirstName = "Nancy "})
            .WithMediaRangeModel("text/html", new RatPack {FirstName = "Nancy fancy pants"})
            .WithView("negotiatedview")
            .WithHeader("X-Custom", "SomeValue");
    };


This demo highlights that if you made a request to "/" in your application by a web browser it will return a specific model with a property name of "Nancy fancy pants", return a view called "negotiatedview" and return a custom header. However, if your API client made a request to "/" it would return a model with "Nancy" and a custom header.  The resulting model would then be serialized into JSON, JSONP, XML or any other variation specified in the Accept header from your client.  This example is possibly contrived somewhat but Nancy supplies conneg from all routes so something like the below would be serialized based on the headers.


    Get["/"] = parameters => {
        var model = MyModel();
        return model;
    };


__No Config__ - To get Nancy up and running there is no config required, no nasty XML files to modify, nothing.  As its host agnostic you don't have to modify anything in web.config to have it running via IIS for example.

__Runs Anywhere__ - As I just mentioned Nancy is host agnostic which means you can run it in IIS, WCF, embedded within a EXE, as a windows service or within a self hosted application. Pretty much everywhere!

__Pipeline Hooks__ - Nancy allows you to modify the pipeline ie.the request and response before and after they are invoked.  One simple example is saving your data at the end of a request.


    protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
    {
        base.RequestStartup(container, pipelines, context);
    
        pipelines.AfterRequest.AddItemToEndOfPipeline(
            (ctx) =>
            {
                var documentSession = container.Resolve<IDocumentSession>();
    
                if (ctx.Response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    documentSession.SaveChanges();
                }
    
                documentSession.Dispose();
            });
    }


Here we configure the AfterRequest delegate to find the IDocumentSession used in the request, save the changes to the database and then dispose of the IDocumentSession (although TinyIOC would actually dispose of this for you).

A more complex example could be that you modify the way the Request.Form is populated on a HTTP POST, it is that extensible and configurable you could do that quite easily.

__No ties to System.Web and a Freely Designed Framework__ - System.Web is the core DLL based in ASP.Net. It contains the whole kitchen sink of the framework so you get everything bundled into your application even if you only use 25% of the possibilities.  Nancy is architected the other way in that there are [numerous plugins][5] that supply additional and alternative functionality.  Nancy is also not bound to any specific implementation or framework and all requests and responses are built from the ground up allowing it to be loosely coupled and free.  This also means that Nancy can run in the .Net client profile environments without the added requirement for .Net full profile that ASP.NET MVC does require.

__Support & Community__ - One of the great things about Nancy is its community and support.  They have a very active [Google group][6] and you'll find loads of help in [Jabbr][7] to get your questions answered ASAP.  There is a real feeling of community and support because people want to spread the good word about Nancy.  It has over 100 contributors to the project but keep in mind the vision, impetus and most of the work is done by 2 guys, [@TheCodeJunkie][8] and [@GrumpyDev][9] not a huge team sitting in Redmond.  One final thing the [swag][10] is a lot more stylish than Microsoft t-shirts :)

##Conclusion
In one of my last [blog posts][11] I described how you could test the full pipeline in ASP.NET Web API because Microsoft don't supply a nice way to do it.  This blog post got the attention of Microsoft and at the time of writing this blog post it has had over 3150 hits and appeared on the home page of ASP.NET.  The core of the code in that post was taken out of Nancy.  So please if you liked what you saw there, give Nancy a try I think you'll find there many benefits described above as well as others I've not mentioned.  It is your duty as software developers to try new things and investigate tools.

So when your next project comes about and your manager says "Ok lets write our new app in ASP.NET MVC" your reactions should be reflected by these animated GIFs.

__Should we really use ASP.NET MVC?__
![Should we use ASP.NET MVC?](http://i.minus.com/imM6lHNgto38I.gif)

__The boss says we can use Nancy!__
![All systems go](http://i.minus.com/i3nPmdfBC2VaH.gif)

__We have our new Nancy app up and running in no time!__
![Up and Running](http://i.minus.com/ie4Om0y5NXhS1.gif)

For more infomation on Nancy checkout the [website][12] and [documentation][13].


  [1]: http://nancyfx.org/
  [2]: https://github.com/grumpydev/TinyIoC
  [3]: http://www.mono-project.com/Main_Page
  [4]: http://www.raspberrypi.org/quick-start-guide
  [5]: http://nuget.org/packages?q=nancy
  [6]: https://groups.google.com/forum/?fromgroups#!forum/nancy-web-framework
  [7]: http://jabbr.net/#/rooms/nancyfx
  [8]: http://twitter.com/TheCodeJunkie
  [9]: http://twitter.com/GrumpyDev
  [10]: http://nancyfx.spreadshirt.net
  [11]: http://blog.jonathanchannon.com/2012/11/29/asp-net-web-api-testing
  [12]: http://nancyfx.org/
  [13]: https://github.com/NancyFx/Nancy/wiki/Documentation

@Partial['blogfooter', Model.MetaData];

@EndSection