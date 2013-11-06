System.Configuration.Abstractions
====================

# What is it?
In most projects, you're going to have some configuration. In .NET projects, it'll probably start in your app.config or web.config file.

However, if you love TDD, you'll likely have notice that all of the built in configuration classes are horribly un-testable. They all revolve around static references to System.Configuration.ConfigurationManager, and don't really have any interfaces, so in every project, you end up wrapping them into something like "IAppSettingsWrapper", in order to write tests.

After writing these wrappers several thousand times, and being inspired by the excellent "System.IO.Abstractions" package, we've put together a standardised set of wrappers around these core framework classes.

# Why Do I Need It?

* Want to mock/stub/whatever out your App/Web.config files? 
* Want to assert that the values from configuration are *really* configuring your application?
* Want to add custom hooks around loading configuration values?
* Want stronger typing?

This is for you.

# Getting Started

The simplest use case is to bind up `IConfigurationManager` to `System.Configuration.Abstractions.ConfigurationManager` in your DI container.
Alternatively, you can use `System.Configuration.Abstractions.ConfigurationManager.Instance` - a property that'll new up a new instance of IConfigurationManager each time it's accessed.

If you want to directly switch out calls to `System.Configuration.ConfigurationManager` in-place, to take advantage of the strongly typed extensions and `IConfigurationInterceptors` you can replace calls to `System.Configuration.ConfigurationManager` with calls to `System.Configuration.Abstractions.ConfigurationManager.Instance` in-line, and your code should function identically.

Lastly, you can just new up an instance of `System.Configuration.Abstractions.ConfigurationManager` anywhere, using its default constructor, and everything'll be just fine.

# Value Added Features!

The `IAppSettingsExtended` interface, which our `AppSettingsExtended` class implements, contains two new methods:

    public interface IAppSettingsExtended
    {
        string AppSetting(string key);
        T AppSetting<T>(string key);
    }
    
These strongly typed "AppSetting" helpers, will convert any primitive types that `Convert.ChangeType` supports. The most obvious use case being int / bool / float / int? / bool? from their string representations - keeping alot of noisy conversions out of your code.

# Extensibility - IConfigurationInterceptors

`IConfigurationInterceptor`'s are hooks that, if registered, allow you to intercept and manipulate the values retrieved from configuration.

To wire up an `IConfigurationInterceptor`, first, implement one, then call the static method `ConfigurationManager.RegisterInterceptors(interceptor);`
Your interceptors are singletons, so keep them free of instance variables, as they are not thread safe.

Interceptors **only** fire when using the `IAppSettingsExtended` `AppSettting` and `AppSetting<T>` methods to ensure compatibility with `System.Configuration.ConfigurationManager`.

# Why would I want interceptors?

An obvious example would be the presence of an appSetting looking like this:

    <add key="my-key" value="{machineName}-something" />
    
You could easily add an interceptor to detect and fill in `{machineName}` from an environmental variable, keeping your configuration free of painful transformations.
There are several other useful scenarios (auditing and logging, substitution, multi-tenancy) that interceptors could be useful in.

# Included, optional interceptors

## ConfigurationSubstitutionInterceptor

The `ConfigurationSubstitutionInterceptor` is bundled with the package, firstly as an example, but also as a useful configuration interceptor.
It supports embedding any appsetting into any other.  Given:

    <add key="key1" value="valueOfOne" />
    <add key="key2" value="{key1}-valueOfTwo" />

With this interceptor registered, this is true:

	var result = ConfigurationManager.AppSetting<string>("key2");
	Console.WriteLine(result); // writes: valueOfOne-valueOfTwo
	
This interceptor will help you simplify transformed web or app config files that rely on similar / repetitive token replacements, by allowing you to override just one value, and have it nested across the rest of your configuration using the interceptor.

# Contributing

Send a pull request with a passing test for any bugs or interesting extension ideas.

# Credits

David Whitney