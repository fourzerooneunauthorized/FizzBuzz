using FizzBuzz.Config;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace FizzBuzz.Tests;


public class AppConfigProviderTests
{

    [TestCaseSource( nameof( GoodCommandLineArgsSource ) )]
    public void TestGetConfig( string[] args )
    {
        AppConfigProvider? configProvider = null;


        Assert.DoesNotThrow( () => configProvider = new AppConfigProvider( new ConfigurationBuilder()
                                                                           .AddCommandLine( args )
                                                                           .Build() ) );

        Assert.That( configProvider, Is.Not.Null );
        AppConfig config = configProvider.GetConfig();
        Assert.That( config, Is.Not.Null );
        Assert.That( config.Rounds, Is.GreaterThan( 0 ) );
        Assert.That( config.Divisors, Is.Not.Null );
        Assert.That( config.Divisors, Is.Not.Empty );
        Assert.That( config.Divisors.Keys.All( k => k > 0 ) );
        Assert.That( config.Divisors.Values.All( v => !string.IsNullOrWhiteSpace( v ) ) );
    }


    private static IEnumerable<string[]> GoodCommandLineArgsSource()
    {
        yield return [ ];
        yield return [ "rounds=100" ];
        yield return [ "divisors:3=Fizz" ];
        yield return [ "matchByContains=true" ];
        yield return [ "rounds=100", "divisors:3=Fizz" ];
        yield return [ "rounds=100", "divisors:5=Buzz" ];
        yield return [ "rounds=100", "divisors:3=Fizz", "divisors:5=Buzz" ];
        yield return [ "rounds=1000", "divisors:3=Fizz", "divisors:567=Buzz Test 123" ];
        yield return [ "rounds=100", "divisors:3=Fizz", "divisors:5=69CEA7E7-5B19-4C09-807C-D971A05928D7" ];
        yield return [ "rounds=100", "divisors:3=Fizz", "divisors:5=Buzz=Zzub" ];
    }


    [TestCaseSource( nameof( BadCommandLineArgsSource ) )]
    public void TestRejectedConfig( string[] args )
    {
        Assert.Throws<AppConfigValidationException>( () =>
        {
            _ = new AppConfigProvider( new ConfigurationBuilder()
                                       .AddCommandLine( args )
                                       .Build() );
        } );
    }


    private static IEnumerable<string[]> BadCommandLineArgsSource()
    {
        yield return [ "rounds=100", "divisors:5=" ]; // divisor replacement text missing
        yield return [ "rounds=0", "divisors:3=Fizz", "notasupportedarg=1" ]; // unknown arg given
        yield return [ "rounds=0", "divisors:3=Fizz" ]; // rounds < 1
        yield return [ "rounds=not a number", "divisors:3=Fizz" ]; // rounds is not a number
        yield return [ "divisors:not a number=Fizz" ]; // divisor arg key not a number
        yield return [ "divisors:0=Fizz" ]; // divisor arg key < 1
        yield return [ "divisors=Fizz" ]; // no divisor index
        yield return [ "rounds=100", "matchByContains=not a bool" ]; // matchByContains not boolean
    }

}
