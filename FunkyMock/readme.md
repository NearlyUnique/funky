# Developer Notes

## State of the code

This is a series of experiments and explorations to see what looks possible and how things might work. Any tests are just demonstrations how how tests might be written rather than real tests. Any production code is equally untested experiments.

At this point the idea seems plausible, the next step is to decide what the output template should look like.

## Inspiration

I've been building interfaces like this by hand because it is still better then eht exiting frameworks. However this can be a hard sell to other team members. After using [moq for go](https://github.com/matryer/moq) for a project and then seeing the video from Stefan Pölz, I was inspired to actually get started.

[Stefan Pölz :: Lets build an incremental source generator with roslyn](https://www.jetbrains.com/dotnet/guide/tutorials/dotnet-days-online-2022/lets-build-an-incremental-source-generator-with-roslyn/) and the handy [source examples](https://github.com/Flash0ver/F0-Talks-SourceGenerators).

Source generators still seem to be more difficult than I'd hope.

## Debugging

Sometimes (certainly with Jetbrains Rider) you will see the occasional "Are you missing an assembly reference" error when trying to use the under development version as a project reference. The only solution to this is, make sure the Generator project has been built, the unit tests should pass (but if the example project still fails) restart your IDE.

Because the generator is loaded at runtime via reflection and witchcraft you cannot set breakpoints easily. The [Microsoft / Roslyn notes](https://github.com/dotnet/roslyn/blob/9dad013b7a3fabeb1b4f36e260ed9c6e3344548e/docs/features/source-generators.cookbook.md#unit-testing-of-generators) really help.

The unit tests are great for asserting the truth but not for debugging, so see Debuggable.cs

## Feature selection

We like simple, there will be interface capabilities that we don't want to support and some that are not supported yet. Any that meet this can be implemented by hand by developers fairly simply in their own code.

## Nuget & Build

Adapted from [How To Build & Publish NuGet Packages With GitHub Actions](https://www.jamescroft.co.uk/how-to-build-publish-nuget-packages-with-github-actions/)

## Critical project setup

From [This infoQ article](https://www.infoq.com/articles/CSharp-Source-Generator/)

Without this the nuget package simply does nothing.

> At the bare minimum you need two projects, one for the source generator itself and one to test it against. For your source generator, you need to make the following additions to the project file.

> First, you need to set the target framework to .NET Standard 2.0. This is required by the C# compiler, so it is not optional. Furthermore, all of your dependencies must likewise be for .NET Standard 2.0 or earlier.
```xml
<TargetFramework>netstandard2.0</TargetFramework>
```
> Then add the CodeAnalysis libraries from NuGet.
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="3.9.0" PrivateAssets="all" />
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.2" PrivateAssets="all" />
</ItemGroup>
```
> Then you are going to want to indicate this is an "analyzer". If you skip this step, your source generator won’t work when deployed as a NuGet package.
```xml
<ItemGroup>
  <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
</ItemGroup>
```
