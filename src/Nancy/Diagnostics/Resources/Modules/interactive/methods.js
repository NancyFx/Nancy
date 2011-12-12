(function (Method) {
    var app = diagnostics.app;

    Method.Model = Backbone.Model.extend({});

    Method.Collection = Backbone.Collection.extend({
        model: Method.Model,
            
        url: function () {
            Nancy.config.basePath + "interactive/providers/" + this.options.providerName;
        }
    });
})(diagnostics.module("method"));