(function (Results, Handlebars) {
    var app = diagnostics.app;

    Results.LayoutManager = new Backbone.LayoutManager({
        fetch: function (path) {
            var done = this.async();

            $.get("@Path['~/_Nancy/interactive/template/']" + path)
					.success(function (contents) { done(contents); })
					.error(function () { done(null); });
        },

        render: function (template, context) {
            return Handlebars.compile(template)(context);
        }
    });

    Results.Views.Result = Backbone.LayoutManager.View.extend({
        template: function () {
            return this.providerName + "/" + this.methodName;
        },

        initialise: function (opts) {
            this.providerName = opts.providerName;
            this.methodName = opts.methodName;
        }
    });

    Results.execute = function (executionContext) {
        console.log("executing: " + executionContext.methodName);
        console.log("Arguments:");
        _.each(executionContext.arguments, function (arg) {
            console.log("Name: " + arg.name + " Value:" + arg.value);
        });

        var resultsView = new Results.Views.Result({
                providerName: executionContext.providerName,
                methodName: executionContext.methodName,
            });
    };

    app.bind("execute", Results.execute);
})(diagnostics.module("results"), Handlebars);