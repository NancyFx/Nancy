(function (Results, Handlebars, $) {
    var app = diagnostics.app;

    Results.LayoutManager = new Backbone.LayoutManager({
        fetch: function (path) {
            var done = this.async();

            $.get(Nancy.config.basePath + "interactive/templates/" + path)
					.success(function (contents) { done(contents); })
					.error(function () { done(null); });
        },

        render: function (template, context) {
            return Handlebars.compile(template)(context);
        }
    });

    Results.Views.Result = Backbone.View.extend({
        el: '#results',

        initialize: function (opts) {
            this.providerName = opts.providerName;
            this.methodName = opts.methodName;
            this.arguments = opts.arguments;
            this.templatePath = this.providerName + "/" + this.methodName;
        },

        render: function () {
            $.ajax({
                    url: Nancy.config.basePath + "interactive/templates/" + this.templatePath,
                    dataType: "text",
                    context: this,
					success: this.renderWithTemplate,
					error: this.renderWithoutTemplate
                });
        },

        renderWithTemplate: function (template) {
            var html = Handlebars.compile(template)({ model: this.model });
            $(this.el).append(html);
        },

        renderWithoutTemplate: function () {
            $(this.el).html(_.jsonreport(this.model));
        }
    });

    Results.execute = function (executionContext) {
        console.log("executing: " + executionContext.providerName + "/" + executionContext.methodName);
        console.log("Arguments:");
        _.each(executionContext.arguments, function (arg) {
            console.log("Name: " + arg.name + " Value:" + arg.value);
        });

        $.ajax({
                url: Nancy.config.basePath + "interactive/providers/" + executionContext.providerName + "/" + executionContext.methodName,
                dataType: "text",
                data: executionContext.arguments,
                success: function (data) {
                    var resultsView = new Results.Views.Result({
                        providerName: executionContext.providerName,
                        methodName: executionContext.methodName,
                        model: data
                    });

                    resultsView.render();
                }
            });
    };

    app.bind("execute", Results.execute);
})(diagnostics.module("results"), Handlebars, $);