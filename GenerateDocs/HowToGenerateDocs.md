# Generating Documentation

We use Microsoft's XML based documentation to document functions and classes.

Documentation:

* [Getting Started](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
* [Example](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#d5-an-example)
* [Specification](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#d5-an-example)

# Building

We use [doxfx](https://github.com/dotnet/docfx) to build the documentaton. 
From the developer command line, install docfx like so

```
dotnet tool install -g docfx
```

To run docfx, 

```
cd GenerateDocs
docfx metadata docfx.json
docfx build docfx.json
```

All docs will be generated into this directory.

# Notes

You need the command line tools for Visual Studio to be installed in order to run `dotnet` commands.

The API documentation tool chain in Visual Studio works like this:

* Write XML tagged comments prefixed with `///` on each line
* Visual Studio will create an XML document for the APIs in your project, as a build step
* Using `docfx`, we do not need the intermediary steps. It is done for us. However, it is useful to have Visual Studio parse XML comments and show warnings.

If you want to build the raw XML file for a Unity project, it requires several steps.

First, _XML Documentation File_ needs to be enabled in the project settings. 
Right-click properties, Build tab, Output -> Make sure _XML Documentation File_ is checked.

If you are using Unity, project properties are hidden by default. First, you need to enable project files in Unity. 
This is done in Preferences -> External Tools. Make sure you are using Visual Studio and that 
_generate .csproj files_ is enabled for local and embedded packages. Second, you must 
enable project properties in Visual Studio. In Tools -> Options -> Unity Plugin, make sure 

* _Disable Full Build of Projects_ is false
* _Access to Project Properties_ is true


