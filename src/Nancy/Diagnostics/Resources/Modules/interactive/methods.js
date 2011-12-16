(function (Method) {
    var app = diagnostics.app;

    Method.Model = Backbone.Model.extend({});

    Method.Collection = Backbone.Collection.extend({
        model: Method.Model,

        initialize: function (opts) {
            this.providerName = opts.providerName;
        },

        url: function () {
            return Nancy.config.basePath + "interactive/providers/" + this.providerName;
        }
    });

    Method.Views.List = Backbone.View.extend({
        el: '#methods',

        initialize: function () {
            this.template = $("#list").html();
            this.providerName = this.model.providerName;
        },

        render: function () {
            var methods = this.model.toJSON();

            var html = Handlebars.compile(this.template)({ collection: methods });

            $(this.el).html(html);

            _.each(methods, this.renderItem, this);
        },

        renderItem: function (model) {
            var itemView = new Method.Views.Item({ model: model, providerName: this.providerName });

            this.$('#root').append(itemView.el);
        }
    });

    Method.Views.Item = Backbone.View.extend({
        tagName: 'li',

        events: {
            'click input[type=button]': 'executeMethod'
        },

        initialize: function (args) {
            this.providerName = args.providerName;
            this.app = app;
            this.router = app.router;
            this.template = $("#method").html();
            this.render();
        },

        render: function () {
            var html = Handlebars.compile(this.template)({ model: this.model });
            $(this.el).append(html);
        },

        executeMethod: function () {
            var parameters = this.$("input");

            var executionContext = {};

            executionContext.providerName = this.providerName;
            executionContext.methodName = this.model.MethodName;
            executionContext.arguments = [];

            _.each(parameters, function (input) {
                if (input.type !== "submit" && input.type !== "button") {
                    executionContext.arguments.push({ name: input.id, value: this.$(input).val() });
                }
            }, this);

            this.app.trigger("execute", executionContext);
        }
    });
})(diagnostics.module("method"));