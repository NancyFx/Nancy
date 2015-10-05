<html>
    <head>
        <title>Razor View Engine Demo</title>
    </head>
    <body>
        <h1>VB razor helpers</h1>
    
        @Render("Render from page helper")
    
        @VBCodeHelper.Render("Hello world")
    </body>
</html>

@Helper Render(text as String)
    @<p>@text</p>
End Helper