<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Routes</title>
	<link rel="stylesheet" href='@Url.Content("~/content/main.css")' type="text/css" media="screen"/>
</head>
<body>

	<h1>Routes (VB)</h1>
	<p>Below is a list of all of the currently registered routes in this application</p>
	<ul>
        @For Each routeModule As Object In Model
            For Each route As Object In routeModule.Value
				@<li><a href='@Url.Content("~" + route.Item2.Path)'>@route.Item2.Path</a></li>
			Next
		Next

        <li><a href='thiswillgivea404'>404</a></li>
        <li><a href='@Url.Content("~/content/face.png")'>An image</a></li>
        <li><a href='@Url.Content("~/moo/face.png")'>An image using alternative content convention</a></li>
        <li><a href='@Url.Content("~/content/main.css")'>CSS</a></li>
        <li><a href='@Url.Content("~/content/scripts.js")'>Some javascript</a></li>
	</ul>

</body>
</html>