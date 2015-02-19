namespace Nancy.Tests.Resources.Views
{
    public static class SuperSimpleViewEngineSampleContent
    {
        public static string testFileInput = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>@Model.Title</title>
</head>
<body>
	<h1>@Model.Title</h1>

	<p>Hello there @Model.Name!</p>

	<p>@Model.Complex.Item1;. @Model.Complex.Item2;.</p>

	<h2>Users:</h2>
	@IfNot.HasUsers
	<p>No users found</p>
	@EndIf

	@If.HasUsers
	<ul id=""users"">
		@Each.Users
		<li>@Current.Name - @Current.Age</li>
		@EndEach
	</ul>
	@EndIf

	<h2>Admins:</h2>
	@IfNot.HasAdmins
	<p>No admin users found</p>
	@EndIf

	@If.HasAdmins
	<ul id=""admins"">
		@Each.Admins
		<li>@Current</li>
		@EndEach
	</ul>
	@EndIf
</body>
</html>
";

        public static string ExpectedOutput = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>Demonstration of Nancy's SuperSimple ViewEngine</title>
</head>
<body>
	<h1>Demonstration of Nancy's SuperSimple ViewEngine</h1>

	<p>Hello there Frankie!</p>

	<p>This is a nested property. Oh yes it is.</p>

	<h2>Users:</h2>
	

	
	<ul id=""users"">
		
		<li>Bob Smith - 27</li>
		
		<li>Jim Jones - 42</li>
		
		<li>Bill Bobson - 78</li>
		
	</ul>
	

	<h2>Admins:</h2>
	
	<p>No admin users found</p>
	

	
</body>
</html>
";
    }
}
