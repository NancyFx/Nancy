(function (Provider) {
    var app = diagnostics.app;

    Provider.Model = Backbone.Model.extend({ });

    Provider.Collection = Backbone.Collection.extend({
        model: Provider.Model,
            
        url: function () {
            return Nancy.config.basePath + "interactive/providers/";
        }
    });

    Provider.Views.List = Backbone.View.extend({
        el: '#container',

        initialize: function () {
            this.template = $("#providers-list").html();
        },

        render: function () {
            var html = Handlebars.compile(this.template)({ providers: this.model.toJSON() });
            $(this.el).append(html);
        }
    });

})(diagnostics.module("provider"))