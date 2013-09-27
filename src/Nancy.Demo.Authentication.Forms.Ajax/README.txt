This demo is intended to supplement the official Nancy documentation which doesn't cover
scenarios where you don't want to return the result of Login requests as-is.

The crux of this demo is the Javascript in "~/Views/index.cshtml" which uses the jQuery
cookies plugin to set the "_ncfa" cookie to the encoded and encrypted GUID representing
the currently logged in user.