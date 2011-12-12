(function (Results) {
    var app = diagnostics.app;

    Results.execute = function (target) {
        console.log("executing");
    };

    app.bind("execute", Results.execute);
})(diagnostics.module("results"));