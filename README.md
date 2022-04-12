# rwb196884/pulumi

This repository contains a sequence of branches, each step adding additional features to a `pulumi` project/stack
showing a working example being built up from scratch.

## Prerequisites

Pulimi, Azure, and .NET.

Pulumi is most easily installed via `chocolatey` https://chocolatey.org/install
```
> choco list --localonly
> choco outdated
> choco upgrade all
> choco install pulumi
```

https://phoenixnap.com/blog/what-is-pulumi

You also need an account on https://www.pulumi.com/ and the command line needs to get logged in to this:
```
> pulumi login
```

Azure CLI is at: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli
Again, this needs to log in on the command line:
```
> az login
```

Note that Pulumi C# projects may fail to restore packages if you have any NuGet feeds defined.
It is simplest to edit `AppData\Roaming\NuGet\NuGet.config` to comment out everything
other than `nuget.org`.

## 01 Hello World

Open a DOS prompt and cd to here.
```
> mkdir rwb196884
> mkdir rwb196884
> pulumi new azure-csharp
```

Now check that it builds, but do not deploy (choose `no`) because the default template contains something.

Open the project and save the `.sln`.

The _floating_ pacckage version numbers don't work with VisualStudio.
Open the NuGet package manager, remove the redundant reference to `Pulimi` 
(it is a depencency of `Pulumi.AzureNative`) and update `Pulumi.AzureNative`
to the latest version (1.62.0 at the time of writing). It will now build.

Add a `namespace` to `Program.cs` and `MyStack.cs`.

We can see that `Console.Writeline` and `Pulumi.Log.Info` both produce output
which can be seen both in the DOS window when running the `pulumi` command
and inthe Pulumi website.
This is likely to be useful for debugging becauase the programme is not really 
a programme and can't be run let alone debugged in VisualStudio.

## That's all folks

Now procees to branch sequence-02