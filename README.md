# TakNotify - Amazon SES Provider

The `TakNotify.Provider.AmazonSES` is a `TakNotify` provider that sends email notification via Amazon Simple Email Service.

## NuGet Package

The `TakNotify.Provider.AmazonSES` library is available as a NuGet package at https://www.nuget.org/packages/TakNotify.Provider.AmazonSES.

It can be installed easily via the `Manage NuGet Packages` menu in Visual Studio or by using the `dotnet add package` command in command line:

```powershell
dotnet add package TakNotify.Provider.AmazonSES
```

## Build the source code

If for some reasons you need to build the `TakNotify.Provider.AmazonSES` library from the code yourself, you can always use the usual `dotnet build` command because basically it's just a .NET Core class library:

```powershell
dotnet build .\src\TakNotify.Provider.AmazonSES
```

However, we recommend you to just use the `build.ps1` script because it will not only help you to build the code, but also publish it into a ready to use NuGet package.

```powershell
.\build.ps1
```

## Usage

There are two steps that you need to do to send email notification via Amazon SES with this library:
1. register the provider
2. send the email notification

### Register the provider

In the set up process, you can use `AmazonSESProvider` as the provider type and `AmazonSESOptions` as the provider options type.

In a .NET Core application, you can instantiate the provider and add it to the `INotification` object with something like this:

```c#
var provider = new AmazonSESProvider(
    new AmazonSESOptions{
        AccessKey = "",
        SecretKey = "",
        RegionEndpoint = ""
    }, 
    LoggerFactory.Create(config => config.AddConsole())
);

notification.AddProvider(provider);
```

If you work with an ASP.NET Core application, you can easily register this provider in the `ConfigureServices()` method of the `Startup` class:

```c#
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddTakNotify()
        .AddProvider<AmazonSESProvider, AmazonSESOptions>(options =>
        {
            options.AccessKey = "";
            options.SecretKey = "";
            options.RegionEndpoint = "";
        });
}
```

### Send email notification

After registering the provider into the `INotification` object, you can easily send the email notification by utilizing the `SendEmailWithAmazonSES()` method. It requires an `AmazonSESMessage` object to be passed as parameter:

```c#
var message = new AmazonSESMessage()
{
    FromAddress = "sender@example.com",
    ToAddresses = new List<string> { "user@example.com" },
    Subject = "[TakNotify] Test Email",
    PlainContent = "This email was sent via Amazon SES"
};

var result = await _notification.SendEmailWithAmazonSES(message);
```

You can find how it is used in a working application in the [Samples](https://github.com/TakNotify/Samples) project.

## Dependency and contribution

Internally, this provider uses the [AWSSDK.SimpleEmail](https://www.nuget.org/packages/AWSSDK.SimpleEmail) library to do the work. You can expect all features from the library to be brought into this provider. 

However, because of some limitations, you will find that a lot of them are still missing. This is where we could use your hands in completing the missing pieces. Your contribution will be much appreciated.

## Next

Please refer to the [project page](https://taknotify.github.io/) to get
more details about the usage of this package.
