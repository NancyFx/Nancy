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
        },

        render: function () {
            var methods = this.model.toJSON();

            var html = Handlebars.compile(this.template)({ collection: methods });

            $(this.el).html(html);

            _.each(methods, this.renderItem, this);
        },

        renderItem: function (model) {
            var itemView = new Method.Views.Item({ model: model });

            this.$('ul').append(itemView.el);
        }
    });

    Method.Views.Item = Backbone.View.extend({
        tagName: 'li',

        events: {
            'click input[type=button]': 'executeMethod'
        },

        initialize: function () {
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
            this.app.trigger("execute", { methodName: this.model.MethodName });
        }
    });
})(diagnostics.module("method"));