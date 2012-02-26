$(function () {
    var app = diagnostics.app;

    var Session = diagnostics.module("session");
    var Trace = diagnostics.module("trace");

    var Router = Backbone.Router.extend({
        routes: {
            "": "index",
            ":id": "trace"
        },

        index: function () {
            var sessions = new Session.Collection();
            sessions.fetch().success(function () {
                var list = new Session.Views.List({ model: sessions });
                list.render();
            });
        },

        trace: function (id) {
            var traces = new Trace.Collection({ id: id });
            traces.fetch().success(function () {
                var list = new Trace.Views.List({ model: traces });
                list.render();
            });
        }
    });

    // Start router and trigger first route
    app.router = new Router();
    Backbone.history.start();
});