To create a vsix template

Install extension  Export Template as VSIX wizard

See info at:

http://blogs.msdn.com/b/visualstudio/archive/2010/03/04/creating-and-sharing-project-item-templates.aspx

To maintain references local

A dependencies directory is created in the template and assemblies are copied to that directory.

For example:
copy /Y "$(TargetPath)" "$(SolutionDir)Nancy.Html5BoilerplateTemplate\dependencies\$(TargetFileName)"


*****************************************************************
When runnig the wizard these are the values used, Edit as required

Name
Nancy + Html5BoilerPlate

Description 
Nancy the lightweight web framework plus all the goodness of Html5Boilerplate

Icon Image
Nancy.Html5BoilerplateTemplate\apple-touch-icon.png

Product Name
Nancy.Html5BoilerPlate

Company Name
Nancy Team

Visual Studio Editions
VS 10.0: Ultimate,Premium,Professional,C# Express; 

**************************************************************************
