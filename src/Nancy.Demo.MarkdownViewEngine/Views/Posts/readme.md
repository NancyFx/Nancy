@Master['master']

@Tags
Date: 15/03/2013
Title: Readme
Tags: Nancy,Runtime
@EndTags

@Section['Content']

@Partial['blogheader', Model.MetaData];

# Readme!

## Markdown Viewengine

This Markdown Viewengine allows views to be written in Markdown.

* Full Model support
* Master page support
* Supports HTML within any MD content
* Simple call `return View["Home"]` for Nancy to render your MD file

## Markdown Blog Demo

* Allows for dropping in Markdown files into a directory
* Allows for future post scheduling
* Generates slug automatically
* Uses meta data for date, tags, title
* Allows for custom HTML design

@Partial['blogfooter', Model.MetaData];

@EndSection