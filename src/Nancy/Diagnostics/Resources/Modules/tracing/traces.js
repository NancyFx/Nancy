(function (Trace) {
    var app = diagnostics.app;

    Trace.Model = Backbone.Model.extend({});

    Trace.Collection = Backbone.Collection.extend({
        model: Trace.Model,

        initialize: function (opts) {
            this.id = opts.id;
        },

        url: function () {
            return Nancy.config.basePath + "trace/sessions/" + this.id;
        }
    });

    Trace.Views.List = Backbone.View.extend({
        el: '#main',

        events: {
            'click #back': 'back'
        },

        initialize: function () {
            this.router = app.router;
            this.template = $("#traceList").html();
        },

        render: function () {
            var traces = this.model.toJSON();

            var html = Handlebars.compile(this.template)({ collection: traces });

            $(this.el).html(html);

            _.each(traces, this.renderItem, this);
        },

        renderItem: function (model) {
            var itemView = new Trace.Views.Item({ model: model });

            this.$('#root').append(itemView.el);
        },

        back: function () {
            this.router.navigate("", true);
        }
    });

    Trace.Views.Item = Backbone.View.extend({
        className: 'item-list',

        events: {
            'click': 'viewDetails'
        },

        initialize: function (args) {
            this.template = $("#trace").html();
            this.render();
        },

        render: function () {
            var html = Handlebars.compile(this.template)({ model: this.model });
            $(this.el).append(html);
        },

        viewDetails: function () {
            var details = new Trace.Views.Details({ model: this.model });
            details.render();
        }
    });

    Trace.Views.Details = Backbone.View.extend({
        el: '#details',

        render: function () {
            $(this.el).html("<div class=\"modelreport\">" + _.modelreport(this.model) + "</div>");
        }
    });
})(diagnostics.module("trace"));