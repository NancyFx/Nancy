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

$(function () {
    var app = diagnostics.app;

    var Provider = diagnostics.module("provider");
    var Method = diagnostics.module("method");
    var Results = diagnostics.module("results");

    var Router = Backbone.Router.extend({
        routes: {
            "": "index",
            ":providerName": "methods"
        },

        index: function () {
            var providers = new Provider.Collection();
            providers.fetch().success(function () {
                var list = new Provider.Views.List({ model: providers });
                list.render();
            });
        },

        methods: function (providerName) {
            var methods = new Method.Collection({ providerName: providerName });
            methods.fetch().success(function () {
                var list = new Method.Views.List({ model: methods });
                list.render();
            });
        }
    });

    // Start router and trigger first route
    app.router = new Router();
    Backbone.history.start();
});