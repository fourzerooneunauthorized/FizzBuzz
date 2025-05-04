using FizzBuzz.Config;
using NUnit.Framework;
using System.Collections.Generic;


namespace FizzBuzz.Tests;


public class CommandLineConfigProviderTests
{

    [TestCaseSource( nameof( GoodCommandLineArgsSource ) )]
    public void TestGetConfig( string[] args )
    {
        var configProvider = new CommandLineConfigProvider( args );
        Assert.DoesNotThrow( () => configProvider.GetConfig() );

        AppConfig config = configProvider.GetConfig();
        Assert.That( config, Is.Not.Null );
        Assert.That( config.Rounds, Is.GreaterThan( 0 ) );
        Assert.That( config.Divisors, Is.Not.Null );
        Assert.That( config.Divisors, Is.Not.Empty );
    }


    private static IEnumerable<string[]> GoodCommandLineArgsSource()
    {
        yield return [ ];
        yield return [ "rounds=100" ];
        yield return [ "divisor=3=Fizz" ];
        yield return [ "rounds=100", "divisor=3=Fizz" ];
        yield return [ "rounds=100", "divisor=5=Buzz" ];
        yield return [ "rounds=100", "divisor=3=Fizz", "divisor=5=Buzz" ];
        yield return [ "rounds=1000", "divisor=3=Fizz", "divisor=567=Buzz Test 123" ];
        yield return [ "rounds=100", "divisor=3=Fizz", "divisor=5=69CEA7E7-5B19-4C09-807C-D971A05928D7" ];
        yield return [ "rounds=100", "divisor=3=Fizz", "divisor=5=Buzz=Zzub" ];
        yield return [ "rounds=100", "divisor=3=Fizz", "divisor=5=" ];
    }


    [TestCaseSource( nameof( BadCommandLineArgsSource ) )]
    public void TestRejectedConfig( string[] args )
    {
        var configProvider = new CommandLineConfigProvider( args );
        Assert.Throws<AppConfigLoadingException>( () => configProvider.GetConfig() );
    }


    private static IEnumerable<string[]> BadCommandLineArgsSource()
    {
        yield return [ "rounds=0", "divisor=3=Fizz", "notasupportedarg=1" ]; // unknown arg given
        yield return [ "rounds=0", "divisor=3=Fizz" ]; // rounds < 1
        yield return [ "rounds=not a number", "divisor=3=Fizz" ]; // rounds is not a number
        yield return [ "rounds=100", "rounds=1", "divisor=3=Fizz" ]; // duplicate rounds arg
        yield return [ "divisor=not a number=Fizz" ]; // divisor arg key not a number
        yield return [ "divisor=0=Fizz" ]; // divisor arg key < 1
    }

}
