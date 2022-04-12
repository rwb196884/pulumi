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

We can see that `Console.Writeline` and `Pulumi.Log.Info` both produce output
which can be seen both in the DOS window when running the `pulumi` command
and inthe Pulumi website.
This is likely to be useful for debugging becauase the programme is not really 
a programme and can't be run let alone debugged in VisualStudio.

## 02 SQL Server

Let's create a _resource_!

At this point we need domain knowledge about Azure in order to know what things to create, 
what additional things to create because the things we want depend on them,
and a load of annoying 'security' stuff that prevents things from working to work around.
In this sense the _no domain specific language_ boast is something of a red herring.

Add some configuration values to the _stack_ (the `yml` file) and use the `Config` object to get their values.

Configuration items are key-value pairs. The key is of the format `foo:bar`, and we can only read values where `foo` is the project name
(so in this case `foo` must be `rwb196884`).

If we use an invalid password to create the SQL Server _resource_ then Pulumi still craetes the resource group; it does not tidy up after an error.

Deleting the resource group in the Azure website will confuse Pulumi.
The only possibility is to completely delete the stack and start over.
```
pulumi destroy dev
```

Finally, use the other password and `pulumi up`, then you should be able to connecto the database in SQL Server Management Studio.

## That's all folks

Now procees to branch sequence-03