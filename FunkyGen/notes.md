# Developer Notes

## State of the code

This is a series of experiments and explorations to see what looks possible and how things might work. Any tests are just demonstrations how how tests might be written rather than real tests. Any production code is equally untested experiments.

At this point the idea seems plausible, the next step is to decide what the output template should look like.

## Debugging

Because the generator is loaded at runtime via reflection and witchcraft you cannot set breakpoints easily. The [Microsoft / Roslyn notes](https://github.com/dotnet/roslyn/blob/9dad013b7a3fabeb1b4f36e260ed9c6e3344548e/docs/features/source-generators.cookbook.md#unit-testing-of-generators) really help.

The unit tests are great for asserting the truth but not for debugging, so see Debuggable.cs

## Feature selection

We like simple, there will be interface capabilities that we don't want to support and some that are not supported yet. Any that meet this can be implemented by hand by developers fairly simply in their own code.
