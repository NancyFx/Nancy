(function (Provider) {
    var app = diagnostics.app;

    Provider.Collection = Backbone.Collection.extend({
        url: function () {
            return Nancy.config.basePath + "interactive/";
        }
    });

    Provider.Views.List = Backbone.LayoutManager.View.extend({
        template: "#providers-list",

        serialize: function() {
          return { tweet: this.model.toJSON() };
        }
    });
    
})(diagnostics.module("provider"))