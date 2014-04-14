System.Configuration.Abstractions [![Build status](https://ci.appveyor.com/api/projects/status/ngl0cknxt74bfnve)](https://ci.appveyor.com/project/DavidWhitney/system-configuration-abstractions)
====================

* Introduction
* Installation
* Getting started
* Features
	* Generic typed helper methods for retrieving typed config values
	* Configuration Interceptors
	* Configuration Substitution Interceptor
* Contributing
* Credits

---

In most projects, you're going to have some configuration. In .NET projects, it'll probably start in your app.config or web.config file.

However, if you love TDD, you'll likely have notice that all of the built in configuration classes are horribly un-testable. They all revolve around static references to System.Configuration.ConfigurationManager, and don't really have any interfaces, so in every project, you end up wrapping them into something like "IAppSettingsWrapper", in order to write tests.

After writing these wrappers several thousand times, and being inspired by the excellent "System.IO.Abstractions" package, we've put together a standardised set of wrappers around these core framework classes.

If you want

* to **mock/stub/whatever** out your App/Web.config files
* to assert that the values from configuration are *really* configuring your application
* to **add custom hooks** around loading configuration values
* **stronger typing**

The this is for you.

# Installation

* From source: https://github.com/davidwhitney/System.Configuration.Abstractions
* By hand: https://www.nuget.org/packages/System.Configuration.Abstractions

Via NuGet:

		PM> Install-Package System.Configuration.Abstractions

# Getting started

The simplest use case is to bind up `IConfigurationManager` to `System.Configuration.Abstractions.ConfigurationManager` in your DI container.
Alternatively, you can use `System.Configuration.Abstractions.ConfigurationManager.Instance` - a property that'll new up a new instance of IConfigurationManager each time it's accessed.

If you want to directly switch out calls to `System.Configuration.ConfigurationManager` in-place, to take advantage of the strongly typed extensions and `IConfigurationInterceptors` you can replace calls to `System.Configuration.ConfigurationManager` with calls to `System.Configuration.Abstractions.ConfigurationManager.Instance` in-line, and your code should function identically.

Lastly, you can just new up an instance of `System.Configuration.Abstractions.ConfigurationManager` anywhere, using its default constructor, and everything'll be just fine.

Examples:

```csharp
    // Usages

    // You can use the singleton
    var valString = ConfigurationManager.Instance.AppSettings["stringKey"];
    var valInt = ConfigurationManager.Instance.AppSettings.AppSetting<int>("intKey");

    // You can new up an instance
    var configMgr = new ConfigurationManager();
    var valString2 = configMgr.AppSettings["stringKey"];
    var valInt2 = configMgr.AppSettings.AppSetting<int>("intKey");

    // You can new up an instance with configuration values
    var configMgr3 = new ConfigurationManager(new NameValueCollection {{"stringKey", "hello"}, {"intKey", "123"}});
    var valString3 = configMgr3.AppSettings["stringKey"];
    var valInt3 = configMgr3.AppSettings.AppSetting<int>("intKey");
    
    // You can just switch out calls in place for backwards compatible behaviour
    var old = System.Configuration.ConfigurationManager.AppSettings["stringKey"];
    var @new = ConfigurationManager.Instance.AppSettings["stringKey"];

    // You can wire up to your container
    ninjectContainer.Bind<IConfigurationManager>().ToMethod(()=> return new ConfigurationManager());
```

# Features

## Generic typed helper methods for retrieving typed config values

The `IAppSettingsExtended` interface, which our `AppSettingsExtended` class implements, contains two new methods:

* `string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null);`
* `T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null);`

These strongly typed "AppSetting" helpers, will convert any primitive types that `Convert.ChangeType` supports. The most obvious use case being int / bool / float / int? / bool? from their string representations - keeping alot of noisy conversions out of your code. You can also provide an optional Func<T> which will get invoked if the key you're requesting is not found - otherwise, we'll throw an exception.


The following usage examples illustrate how to use these helpers:
```csharp
    // Before *******************************
    
    var settingThatIsAnInteger = System.Configuration.ConfigurationManager.AppSettings["key"];
    int someInt;
    if (Int32.TryParse(settingThatIsAnInteger, out someInt))
    {
        someInt = 123; // Default
    }

    var settingThatIsABool = System.Configuration.ConfigurationManager.AppSettings["otherKey"];
    bool someBool;
    if (bool.TryParse(settingThatIsAnInteger, out someBool))
    {
        someBool = true; // Default
    }

    // After ********************************
    
    var withADefault = ConfigurationManager.Instance.AppSettings.AppSetting("key", () => 123);
    var withoutADefault = ConfigurationManager.Instance.AppSettings.AppSetting<int>("key");
    
    var worksWithAllPrimatives = ConfigurationManager.Instance.AppSettings.AppSetting<bool>("otherKey");
    var worksWithNullables = ConfigurationManager.Instance.AppSettings.AppSetting<bool?>("otherKey");
    
    var customNotFoundHandler = ConfigurationManager.Instance.AppSettings.AppSetting<bool?>("otherKey", () =>
    {
        throw new MyCustomException();
    });
```    

## IConfigurationInterceptors

`IConfigurationInterceptor`'s are hooks that, if registered, allow you to intercept and manipulate the values retrieved from configuration.
`IConnectionStringInterceptor`'s are hooks that allow you to manipulate connection strings during retrieval in a similar manner.

To wire up an `IConfigurationInterceptor` or `IConnectionStringInterceptor`, first, implement one, then call the static method `ConfigurationManager.RegisterInterceptors(interceptor);`
Your interceptors are singletons and should be thread safe as the same instance could be called across multiple threads concurrently.

Example:

```csharp
	ConfigurationManager.RegisterInterceptors(new ConfigurationSubstitutionInterceptor());
	var result = ConfigurationManager.Instance.AppSettings.AppSetting<string>("key"); // Interceptor executes
	var result2 = ConfigurationManager.Instance.AppSettings["key"]; // Interceptor executes
```

Interceptors fire for both the AppSetting helper, and the standard NameValueCollection methods and indexers. If you want to by-pass interception, access the "Raw" property for the original collection. *This is a change in behaviour in V2*.

### Why would I want interceptors?

An obvious example would be the presence of an appSetting looking like this:
```xml
    <add key="my-key" value="{machineName}-something" />
```    
You could easily add an interceptor to detect and fill in `{machineName}` from an environmental variable, keeping your configuration free of painful transformations.
There are several other useful scenarios (auditing and logging, substitution, multi-tenancy) that interceptors could be useful in.

## Included Interceptors

### ConfigurationSubstitutionInterceptor

The `ConfigurationSubstitutionInterceptor` is bundled with the package, firstly as an example, but also as a useful configuration interceptor.
It supports embedding any appsetting into any other.  Given:
```xml
    <add key="key1" value="valueOfOne" />
    <add key="key2" value="{key1}-valueOfTwo" />
```
With this interceptor registered, this is true:
```csharp
	var result = ConfigurationManager.AppSetting<string>("key2");
	Console.WriteLine(result); // writes: valueOfOne-valueOfTwo
```	
This interceptor will help you simplify transformed web or app config files that rely on similar / repetitive token replacements, by allowing you to override just one value, and have it nested across the rest of your configuration using the interceptor.

# Contributing

Send a pull request with a passing test for any bugs or interesting extension ideas.

# Credits

David Whitney
