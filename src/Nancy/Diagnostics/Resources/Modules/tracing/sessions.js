(function (Session) {
    var app = diagnostics.app;

    Session.Model = Backbone.Model.extend({});

    Session.Collection = Backbone.Collection.extend({
        model: Session.Model,

        url: function () {
            return Nancy.config.basePath + "trace/sessions/";
        }
    });

    Session.Views.List = Backbone.View.extend({
        el: '#main',

        initialize: function () {
            this.template = $("#sessionList").html();
        },

        render: function () {
            var sessions = this.model.toJSON();

            var html = Handlebars.compile(this.template)({ collection: sessions });

            $(this.el).html(html);

            _.each(sessions, this.renderItem, this);
        },

        renderItem: function (model) {
            var itemView = new Session.Views.Item({ model: model });

            $(this.el).append(itemView.el);
        }
    });

    Session.Views.Item = Backbone.View.extend({
        className: "item-list",

        events: {
            'click': 'showSession'
        },

        initialize: function () {
            this.router = app.router;
            this.template = $("#session").html();
            this.render();
        },

        render: function () {
            var html = Handlebars.compile(this.template)({ model: this.model });
            $(this.el).append(html);
        },

        showSession: function () {
            this.router.navigate(this.model.Id, true);
        }
    });
})(diagnostics.module("session"))