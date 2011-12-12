(function (Results) {
    var app = diagnostics.app;

    Results.execute = function (target) {
        console.log("executing: " + target.methodName);
    };

    app.bind("execute", Results.execute);
})(diagnostics.module("results"));