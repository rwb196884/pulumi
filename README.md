# rwb196884/pulumi

This repository contains a sequence of branches, each step adding additional features to a `pulumi` project/stack
showing a working example being built up from scratch.

## Prerequisites.

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

## That's all folks

Now procees to branch sequence-01