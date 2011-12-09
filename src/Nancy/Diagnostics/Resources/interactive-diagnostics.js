Backbone.StaticDiagnosticsView = Backbone.LayoutManager.extend({
    fetch: function (path) {
        var done = this.async();

        $.get(path + ".html")
         .success(function (contents) { done(contents); })
         .error(function () { done(null); });
    },

    render: function (template, context) {
        // If we have a template then render it with handlebars
        if (template) {
            return Handlebars.compile(template)(context);
        }

        // Otherwise fallback to using the json renderer
        return _.jsonreport(context);
    }
});

jQuery(function ($) {
    // Shorten the app namespace
    var app = diagnostics.app;

    var Provider = diagnostics.module("provider");

    var Router = Backbone.Router.extend({
        routes: {
            "": "index"
        },

        fetchProviders: function () {
            var _cache;

            return function (done) {
                if (_cache) {
                    return done(_cache);
                }

                var providers = new Provider.Collection();

                providers.fetch().success(function () {
                    _cache = providers;
                    done(_cache);
                });
            };
        } (),

        index: function () {
            var main = new Backbone.LayoutManager({
                name: "main"
            });

            this.fetchProviders(function (providers) {
                var list = new Provider.Views.List({ providers: providers });

                // Render into the page
                list.render(function (contents) {
                    $("#container").html(contents);
                });
            });
        }
    });

    // Start router and trigger first route
    app.router = new Router();
    Backbone.history.start();
});