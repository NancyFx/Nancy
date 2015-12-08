@ModelType Nancy.Demo.Hosting.Aspnet.Models.RatPack
<html>
<head>
    <title>Razor View Engine Demo</title>
</head>
<body>
    <h1>Hello @Model.FirstName</h1>
    <p>This is a strongly typed vb razor view!</p>
    <img src='@Url.Content("~/content/face.png")' alt="Face"/>
</body>
</html>